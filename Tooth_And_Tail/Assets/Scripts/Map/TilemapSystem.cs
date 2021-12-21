using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Vector3Comparer : IEqualityComparer<Vector3>
{
    bool IEqualityComparer<Vector3>.Equals(Vector3 x, Vector3 y)
    {
        return x == y;
    }

    int IEqualityComparer<Vector3>.GetHashCode(Vector3 obj)
    {
        return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
    }
}

public class Vector3IntComparer : IEqualityComparer<Vector3Int>
{
    bool IEqualityComparer<Vector3Int>.Equals(Vector3Int x, Vector3Int y)
    {
        return x == y;
    }

    int IEqualityComparer<Vector3Int>.GetHashCode(Vector3Int obj)
    {
        return obj.x.GetHashCode() ^ obj.y.GetHashCode() << 2 ^ obj.z.GetHashCode() >> 2;
    }
}
public class TilemapSystem : MonoBehaviour
{
    // 잘못된 범위의 타일을 참조 실패를 리턴하기 위해 사용
    public readonly static Vector2Int Invalid_Range = new Vector2Int(-999, -999);

    public static TilemapSystem Instance;

    [SerializeField] List<GameObject> colliderList = new List<GameObject>();

    [SerializeField] List<GameObject> maps = new List<GameObject>();

    // 현재 씬에 로드된 맵
    GameObject map = null;

    Transform colliders = null;

    Tilemap ground;
    Tilemap wall;
    Tilemap decor;

    Vector3Int[,] tileSpots;

    [SerializeField] List<OutLiner> OutLines = new List<OutLiner>();

    // 디버깅용 변수들

    [SerializeField] GameObject debuggingTextObj;
    [SerializeField] GameObject indicators;
    [SerializeField] GameObject pathIndicator;

    public bool DebuggingText = false;
    public bool DebuggingIndicator = false;

    // ----------------

    // 타일맵의 범위(x축 갯수 y축 갯수)
    public BoundsInt tileBounds;

    PathFinder pathFinder;

    bool startVectorFieldLoading = false;

    // 타일의 갯수, 사이즈 등을 기반으로 타일을 생성한다.
    void Awake()
    {
        if (Instance == null)
        {
            if (GameManager.Instance.CurGameMode == GameMode.Multi || GameManager.Instance.CurGameMode == GameMode.TimeAttack)
                map = Instantiate(maps[2], Vector3.zero, Quaternion.identity);
            //map = Instantiate(maps[Random.Range(0, 3)], Vector3.zero, Quaternion.identity);
            else if (GameManager.Instance.CurGameMode == GameMode.Tutorial)
                map = Instantiate(maps[3], Vector3.zero, Quaternion.identity);
            else
                map = Instantiate(maps[4], Vector3.zero, Quaternion.identity);
            map.transform.position = Vector3.zero;
            ground = map.transform.Find("Ground").GetComponent<Tilemap>();
            wall = map.transform.Find("Wall").GetComponent<Tilemap>();
            decor = map.transform.Find("Decor").GetComponent<Tilemap>();

            Instance = this;
            ground.CompressBounds();
            tileBounds = ground.cellBounds;

            CreateGrid();

            pathFinder = new PathFinder();

            pathFinder.Indicator = pathIndicator;
            pathFinder.DebuggingText = DebuggingText;

            pathFinder.DebuggingIndicator = DebuggingIndicator;

            colliders = transform.Find("Colliders");
            pathFinder.Initialize(tileSpots, tileBounds.size.x, tileBounds.size.y);
        }
        else
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (pathFinder.VectorFieldLoader != null)
        {
            // pathFinder의 벡터필드로딩 스레드가 실행중이라면 중지시킨다.
            if (pathFinder.VectorFieldLoader.IsAlive)
            {
                pathFinder.StopThread();
            }
        }
    }

    void Update()
    {
        if (!startVectorFieldLoading)
        {
            pathFinder.VectorFieldLoading();
            startVectorFieldLoading = true;
        }
    }

    // 타일의 층을 반환하는 함수
    public int GetTileElevation(Vector3 worldPos)
    {
        Vector3Int gridPos = ground.WorldToCell(worldPos);
        ElevationTile tile = (ElevationTile)ground.GetTile(gridPos);
        if (!ReferenceEquals(tile, null))
            return tile.Elevation;
        else return -1;
    }

    public bool IsLoadedVectorField()
    {
        return pathFinder.IsLoadedVectorField;
    }

    // gridPos는 WorldToCell로 나온 값
    public int GetTileElevation(ref Vector3Int cellPos)
    {
        ElevationTile tile = (ElevationTile)ground.GetTile(cellPos);
        if (!ReferenceEquals(tile, null))
            return tile.Elevation;
        else return -1;
    }

    // 해당 위치의 타일의 타입(벽, 계단, 물, 땅)을 반환한다.
    public TileType GetTileType(Vector3 worldPos)
    {
        Vector3Int gridPos = wall.WorldToCell(worldPos);
        if (decor.HasTile(gridPos))
            return ((ElevationTile)decor.GetTile(gridPos)).TileType;
        if (wall.HasTile(gridPos))
            return ((ElevationTile)wall.GetTile(gridPos)).TileType;
        else if (ground.HasTile(gridPos))
            return ((ElevationTile)ground.GetTile(gridPos)).TileType;
        return TileType.Error;
    }

    // cellPos는 WorldToCell로 나온 값
    public TileType GetTileType(Vector3Int cellPos)
    {
        if (decor.HasTile(cellPos))
            return ((ElevationTile)decor.GetTile(cellPos)).TileType;
        else if (wall.HasTile(cellPos))
            return ((ElevationTile)wall.GetTile(cellPos)).TileType;
        else if (ground.HasTile(cellPos))
            return ((ElevationTile)ground.GetTile(cellPos)).TileType;
        return TileType.Error;
    }

    // 해당 위치의 타일이 빌딩 건설이 가능한 타일인지 확인
    public bool IsBuildableTile(Vector3 worldPos)
    {
        if (GetTileType(worldPos) != TileType.Ground)
        {
            return false;
        }

        if (GetTile(worldPos).Height >= 1 || GetTile(worldPos).occupier != null)
        {
            return false;
        }

        return true;
    }

    public bool IsBuildableTile(Vector3Int cellPos)
    {
        if (GetTile(cellPos) == null)
            return false;

        if (GetTileType(cellPos) != TileType.Ground)
            return false;

        if (GetTile(cellPos).Height >= 1 || GetTile(cellPos).occupier != null)
            return false;

        return true;
    }

    public bool IsInBoundsTile(Vector3Int cellPos)
    {
        if (wall.HasTile(cellPos) == false && ground.HasTile(cellPos) == false)
        {
            return false;
        }
        return true;
    }

    public bool IsInBoundsTile(Vector2Int gridPos)
    {
        BoundsInt WallBounds = wall.cellBounds;

        if ((gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= WallBounds.size.x || gridPos.y >= WallBounds.size.y) &&
            gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= tileBounds.size.x || gridPos.y >= tileBounds.size.y)
        {
            return false;
        }

        return true;
    }

    public bool IsInBoundsTile(Vector3 worldPos)
    {
        Vector3Int cellPos = WorldToCellPos(worldPos);

        return IsInBoundsTile(cellPos);
    }

    // cellPos는 WorldToCellPos로 변환된 좌표
    public bool IsWalkableTile(Vector3Int cellPos)
    {
        Vector2Int gridPos = new Vector2Int(cellPos.x + -tileBounds.xMin, cellPos.y + -tileBounds.yMin);

        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= tileBounds.size.x || gridPos.y >= tileBounds.size.y ||
            pathFinder.TileNodes[gridPos.x, gridPos.y] == null)
        {
            return false;
        }

        return pathFinder.TileNodes[gridPos.x, gridPos.y].Height == 0;
    }

    // gridPos는 WorldToTilePos로 변환된 좌표
    public bool IsWalkableTile(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= tileBounds.size.x || gridPos.y >= tileBounds.size.y)
            return false;
        return pathFinder.TileNodes[gridPos.x, gridPos.y].Height == 0;
    }

    public bool IsWalkableTile(Vector3 worldPos)
    {
        Vector3Int cellPos = WorldToCellPos(worldPos);

        if (ground.HasTile(cellPos) == false)
        {
            return false;
        }

        Vector2Int gridPos = WorldToTilePos(worldPos);

        return pathFinder.TileNodes[gridPos.x, gridPos.y].Height == 0;
    }

    public bool IsWalkableTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= tileBounds.size.x || y >= tileBounds.size.y)
            return false;
        return pathFinder.TileNodes[x, y].Height == 0;
    }

    void AddOutLine(int i, int j, OutLineDir dir)
    {
        var outLine = PoolManager.Instance.PullOutLine(dir);
        OutLines.Add(outLine.GetComponent<OutLiner>());
        outLine.transform.position = pathFinder.TileNodes[i, j].worldPosition;
    }

    public void SetOutLine()
    {
        if (OutLines.Count > 0)
        {
            for (int i = 0; i < OutLines.Count; i++)
            {
                OutLines[i].ReturnPool();
            }

            OutLines.Clear();
        }

        Camp myCamp = GameManager.Instance.CommanderList[0];

        for (int i = 0; i < tileBounds.size.x; i++)
        {
            for (int j = 0; j < tileBounds.size.y; j++)
            {
                if (pathFinder.TileNodes[i, j].camp == myCamp)
                {
                    if (i == 0)
                        AddOutLine(i, j, OutLineDir.LD);
                    if (i == tileBounds.size.x - 1)
                        AddOutLine(i, j, OutLineDir.RU);
                    if (j == 0)
                        AddOutLine(i, j, OutLineDir.RD);
                    if (j == tileBounds.size.y - 1)
                        AddOutLine(i, j, OutLineDir.LU);

                    if (i != tileBounds.size.x - 1 && pathFinder.TileNodes[i + 1, j].camp != myCamp)
                    {
                        AddOutLine(i, j, OutLineDir.RU);
                    }
                    if (i != 0 && pathFinder.TileNodes[i - 1, j].camp != myCamp)
                    {
                        AddOutLine(i, j, OutLineDir.LD);
                    }
                    if (j != tileBounds.size.y - 1 && pathFinder.TileNodes[i, j + 1].camp != myCamp)
                    {
                        AddOutLine(i, j, OutLineDir.LU);
                    }
                    if (j != 0 && pathFinder.TileNodes[i, j - 1].camp != myCamp)
                    {
                        AddOutLine(i, j, OutLineDir.RD);
                    }
                }
            }
        }
    }

    // 건물의 타일 점령 함수이다. 
    // occupier - 점령건물, height - 이동 가능 여부
    // bigSize - true면 4칸차지, false면 1칸 차지
    // occupy - 타일 점령 여부
    public void OccupyTile(Character occupier, int height, bool bigSize, bool occupy)
    {
        Vector2Int gridPos = WorldToTilePos(occupier.transform.position);
        pathFinder.TileNodes[gridPos.x, gridPos.y].Occupy(occupier, height, occupy);

        if (bigSize)
        {
            pathFinder.TileNodes[gridPos.x + 1, gridPos.y + 1].Occupy(occupier, height, occupy);

            pathFinder.TileNodes[gridPos.x, gridPos.y + 1].Occupy(occupier, height, occupy);

            pathFinder.TileNodes[gridPos.x + 1, gridPos.y].Occupy(occupier, height, occupy);
        }
    }

    // gridPos는 WorldToCellPos로 나온 값
    public bool HasTile(Vector3Int cellPos)
    {
        return ground.HasTile(cellPos);
    }

    // gridPos는 WorldToCellPos로 나온 값
    public Vector3 CellToWorldPos(Vector3Int cellPos)
    {
        return ground.CellToWorld(cellPos);
    }

    public Vector3Int WorldToCellPos(Vector3 worldPos)
    {
        return ground.WorldToCell(worldPos);
    }

    // 월드 포지션을 내부 타일노드상의 2차원 배열의 좌표로 반환한다.
    public Vector2Int WorldToTilePos(Vector3 worldPos)
    {
        Vector3Int gridPos = WorldToCellPos(worldPos);

        gridPos.x -= tileBounds.xMin;
        gridPos.y -= tileBounds.yMin;

        return new Vector2Int(gridPos.x, gridPos.y);
    }

    public Vector3Int GridPosToCellPos(Vector2Int gridPos)
    {
        gridPos.x += tileBounds.xMin;
        gridPos.y += tileBounds.yMin;

        return new Vector3Int(gridPos.x, gridPos.y, 0);
    }

    public Vector2Int CellPosToGridPos(Vector3Int cellPos)
    {
        cellPos.x -= tileBounds.xMin;
        cellPos.y -= tileBounds.yMin;

        return new Vector2Int(cellPos.x, cellPos.y);
    }

    // worldPos의 위치를 타일노드의 위치로 변환해서
    // 해당하는 자리에 있는 타일 노드를 반환한다.
    public TileNode GetTile(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToTilePos(worldPos);

        if (gridPos.x < 0 || gridPos.x >= ground.cellBounds.size.x || gridPos.y < 0 || gridPos.y >= ground.cellBounds.size.y)
            return null;

        return pathFinder.TileNodes[gridPos.x, gridPos.y];
    }

    public TileNode GetTile(Vector3Int cellPos)
    {
        Vector3 worldPos = ground.CellToWorld(cellPos);

        TileNode node = GetTile(worldPos);

        return node;
    }

    // gridPos는 WorldToTilePos로 변환된 좌표
    public TileNode GetTile(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= ground.cellBounds.size.x || gridPos.y < 0 || gridPos.y >= ground.cellBounds.size.y)
            return null;
        return pathFinder.TileNodes[gridPos.x, gridPos.y];
    }

    // 타일맵 초기화 함수
    public void CreateGrid()
    {
        tileSpots = new Vector3Int[tileBounds.size.x, tileBounds.size.y];

        for (int x = tileBounds.xMin, i = 0; i < (tileBounds.size.x); x++, i++)
        {
            for (int y = tileBounds.yMin, j = 0; j < tileBounds.size.y; y++, j++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);

                if (ground.HasTile(gridPos) && wall.HasTile(gridPos) == false)
                {
                    tileSpots[i, j] = gridPos;
                }
                else
                {
                    gridPos.z = 1;
                    tileSpots[i, j] = gridPos;
                }
            }
        }
    }

    // from에서 to로 가는 에이스타 경로 얻어오기
    public List<TileNode> GetPath(Vector3 from, Vector3 to)
    {
        List<TileNode> path = new List<TileNode>();

        Vector3Int startPos = ground.WorldToCell(from);
        Vector3Int endPos = ground.WorldToCell(to);

        path = pathFinder.CreatePath(tileSpots, new Vector2Int(startPos.x, startPos.y), new Vector2Int(endPos.x, endPos.y), 1000);
        if (path != null)
        {
            if (path.Count <= 0)
                return null;
        }

        return path;
    }

    // to가 from에서 타일좌표로 range 이상 떨어져 있는지 여부
    public Vector2Int RangeInObject(Vector3 from, Vector3 to, int range)
    {
        Vector3Int gridFrom, gridTo;

        gridFrom = ground.WorldToCell(from);
        gridTo = ground.WorldToCell(to);

        int distX = Mathf.Abs(gridFrom.x - gridTo.x);
        int distY = Mathf.Abs(gridFrom.y - gridTo.y);

        if (distX < range && distY < range)
            return new Vector2Int(distX, distY);
        return Invalid_Range;
    }

    // 프로퍼티 정의 부분
    public Tilemap Ground
    {
        get { return ground; }
    }

    public Tilemap Wall
    {
        get { return wall; }
    }

    public Tilemap Decor
    {
        get { return decor; }
    }

    public GameObject Map
    {
        get { return map; }
    }

    public List<GameObject> Maps
    {
        get { return maps; }
    }

    public Transform Colliders
    {
        get { return colliders; }
    }

    public PathFinder PathFinder
    {
        get { return pathFinder; }
    }

    public List<GameObject> ColliderList
    {
        get { return colliderList; }
    }

    public GameObject DebuggingTextObj
    {
        get { return debuggingTextObj; }
    }

    public GameObject Indicators
    {
        get { return indicators; }
    }

}