using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using TMPro;

public enum Diagonal
{
    Up, Down, Left, Right, End
}

public class PathFinder
{
    TileNode[,] tileNodes;
    GameObject indicator;
    bool debuggingText = false;
    bool debuggingIndicator = false;
    bool isLoadedVectorField = false;

    Thread vectorFieldLoader = null;
    bool threadExit = false;

    public void ResetSteps()
    {
        int cols = TilemapSystem.Instance.tileBounds.size.x;
        int rows = TilemapSystem.Instance.tileBounds.size.y;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                tileNodes[i, j].steps = 99999;
            }
        }
    }

    // 타일1(X1,Y1)에서 타일2(X2,Y2)로 가는 방향이 대각선(상하좌우)상에 있는지 검사
    public Diagonal IsDiagonal(int X1, int Y1, int X2, int Y2)
    {
        // 상
        if (X2 == X1 + 1 && Y2 == Y1 + 1)
        {
            return Diagonal.Up;
        }
        // 하
        if (X2 == X1 - 1 && Y2 == Y1 - 1)
        {
            return Diagonal.Down;
        }
        // 좌
        if (X2 == X1 - 1 && Y2 == Y1 + 1)
        {
            return Diagonal.Left;
        }
        // 우
        if (X2 == X1 + 1 && Y2 == Y1 - 1)
        {
            return Diagonal.Right;
        }
        return Diagonal.End;
    }

    // 대각선 경로가 벽을 관통해서 지나가는지 판단
    public bool IsValidDiagonal(int X1, int Y1, int X2, int Y2)
    {
        Diagonal diagonal = IsDiagonal(X1, Y1, X2, Y2);

        // 대각선이 아니면 검사할 필요가 없기 때문에 true반환
        if (diagonal == Diagonal.End)
            return true;

        switch (diagonal)
        {
            case Diagonal.Up:
                if (TilemapSystem.Instance.IsWalkableTile(X2 - 1, Y2) && TilemapSystem.Instance.IsWalkableTile(X2, Y2 - 1))
                {
                    return true;
                }
                break;
            case Diagonal.Down:
                if (TilemapSystem.Instance.IsWalkableTile(X2 + 1, Y2) && TilemapSystem.Instance.IsWalkableTile(X2, Y2 + 1))
                {
                    return true;
                }
                break;

            case Diagonal.Left:
                if (TilemapSystem.Instance.IsWalkableTile(X2, Y2 - 1) && TilemapSystem.Instance.IsWalkableTile(X2 + 1, Y2))
                {
                    return true;
                }
                break;

            case Diagonal.Right:
                if (TilemapSystem.Instance.IsWalkableTile(X2 - 1, Y2) && TilemapSystem.Instance.IsWalkableTile(X2, Y2 + 1))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    public void VectorFieldLoading()
    {
        int cols = TilemapSystem.Instance.tileBounds.size.x;
        int rows = TilemapSystem.Instance.tileBounds.size.y;

        // 스레드를 이용하여 벡터필드 정보를 로딩
        vectorFieldLoader = new Thread(new ThreadStart(() =>
        {
            Vector3 direction = Vector3.zero;

            int min = 999999;

            bool check = false;

            // 벡터필드 생성
            for (int i = 0; i < cols; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    if (threadExit)
                    {
                        break;
                    }

                    // 만약 갈 수 없는 타일이 건물같은 동적 지형이라면 벡터필드를 생성한다.
                    if (tileNodes[i, j].Height.Equals(1) && ReferenceEquals(tileNodes[i, j].occupier, null))
                        continue;

                    Vector2Int key = new Vector2Int(i, j);

                    // 해당 타일을 0걸음으로 시작해서 다른 모든 타일들에 대해 발걸음 수를 세팅한다.
                    tileNodes[i, j].SetSteps();
                    tileNodes[i, j].VectorField.Add(key, Vector3.zero);

                    // tileNodes[i,j]로부터 모든 타일에 대한 발걸음 수가 세팅 된 후에 발걸음에 따라 나아갈 방향을 결정한다.
                    for (int col = 0; col < cols; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        {
                            if (tileNodes[col, row] == tileNodes[i, j])
                                continue;

                            direction = Vector3.zero;
                            min = 999999;
                            check = false;

                            // 이웃한 노드중 발걸음 수가 가장 적은 노드의 위치를 가리킨다.
                            for (int nIdx = 0; nIdx < tileNodes[col, row].Neighbors.Count; ++nIdx)
                            {
                                TileNode neighbor = tileNodes[col, row].Neighbors[nIdx];

                                // 타일 경로 설정 시 대각선을 모두 허용하면 벽의 모서리를 뚫고 지나갈 수 있기 때문에
                                // tile에서 이웃한 타일이 대각선상에 있는지 검사하고 갈 수 있는 대각선인지 검사한다.
                                if (!IsValidDiagonal(tileNodes[col, row].col, tileNodes[col, row].row, neighbor.col, neighbor.row))
                                    continue;
                                if (min > neighbor.steps)
                                {
                                    direction = neighbor.worldPosition;
                                    min = neighbor.steps;
                                    check = true;
                                }
                            }
                            if (check)
                                tileNodes[col, row].VectorField.Add(key, direction);
                        }
                    }
                    // 다음 타일에 대한 발걸음을 세팅해야 하므로 현재 세팅된 발걸음 정보를 리셋해준다.
                    ResetSteps();
                }
                if (threadExit)
                {
                    Debug.Log("스레드 종료");
                    break;
                }
            }
            isLoadedVectorField = true;
        }));
        vectorFieldLoader.Start();
    }

    public void Initialize(Vector3Int[,] grid, int cols, int rows)
    {
        tileNodes = new TileNode[cols, rows];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // 각 타일의 정보 업데이트
                tileNodes[i, j] = new TileNode(grid[i, j].x, grid[i, j].y, grid[i, j].z);
                tileNodes[i, j].worldPosition = TilemapSystem.Instance.Ground.GetCellCenterWorld(new Vector3Int(grid[i, j].x, grid[i, j].y, grid[i, j].z));
                tileNodes[i, j].col = i;
                tileNodes[i, j].row = j;

                TileType tileType = TilemapSystem.Instance.GetTileType(tileNodes[i, j].worldPosition);

                // 계단의 경우 유닛이 갈 수 있기 때문에 Height를 0으로 바꾼다.
                if (tileType == TileType.Ramp)
                {
                    tileNodes[i, j].Height = 0;
                }

                // 현재 타일이 데코가 있는 타일이고 지나갈 수 없는 타일이라면 콜라이더를 생성한다.
                Vector3Int decorCellPos = TilemapSystem.Instance.Decor.WorldToCell(tileNodes[i, j].worldPosition);
                if (TilemapSystem.Instance.Decor.HasTile(decorCellPos))
                {
                    if (((ElevationTile)TilemapSystem.Instance.Decor.GetTile(decorCellPos)).TileType == TileType.Wall)
                    {
                        tileNodes[i, j].Height = 1;
                        GameObject.Instantiate(TilemapSystem.Instance.ColliderList[(int)TileDir.Decor], tileNodes[i, j].worldPosition, Quaternion.identity, TilemapSystem.Instance.Colliders);
                    }
                }

                Vector3Int wallCellPos = TilemapSystem.Instance.Wall.WorldToCell(tileNodes[i, j].worldPosition);

                // 벽의 위치와 크기에 맞춰 콜라이더를 생성해 준다.
                if (TilemapSystem.Instance.Wall.HasTile(wallCellPos))
                {
                    ElevationTile tile = (ElevationTile)TilemapSystem.Instance.Wall.GetTile(wallCellPos);
                    if (tile != null && tile.TileType == TileType.Wall)
                    {
                        if (tile.TileDir != TileDir.D_OUT)
                        {
                            var colliderObj = GameObject.Instantiate(TilemapSystem.Instance.ColliderList[(int)tile.TileDir], tileNodes[i, j].worldPosition, Quaternion.identity, TilemapSystem.Instance.Colliders);
                            colliderObj.transform.localScale = TilemapSystem.Instance.Map.transform.localScale;
                        }
                    }
                }

                // 디버깅용 오브젝트 생성
                if (debuggingText)
                {
                    GameObject text = GameObject.Instantiate(TilemapSystem.Instance.DebuggingTextObj, tileNodes[i, j].worldPosition, Quaternion.identity, TilemapSystem.Instance.Indicators.transform);
                    text.GetComponent<TextMeshPro>().text = i.ToString() + " , " + j.ToString();
                }
                if (debuggingIndicator && i == 5 && j == 66)
                {
                    GameObject indicator = GameObject.Instantiate(this.indicator, tileNodes[i, j].worldPosition, Quaternion.identity, TilemapSystem.Instance.Indicators.transform);
                    if (tileType == TileType.Wall || tileNodes[i, j].Height == 1)
                    {
                        indicator.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else if (tileType == TileType.Ramp)
                    {
                        indicator.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                }
            }
        }

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                tileNodes[i, j].AddNeighbors(tileNodes, i, j);
            }
        }

    }

    public void StopThread()
    {
        threadExit = true;
        vectorFieldLoader.Join();
    }

    // Start is called before the first frame update
    bool IsValidPath(Vector3Int[,] grid, TileNode start, TileNode end)
    {
        if (start == null || end == null || end.Height >= 1)
        {
            return false;
        }
        return true;
    }

    // 에이스타로 시작점부터 끝점까지의 경로를 찾아 리턴하는 함수
    public List<TileNode> CreatePath(Vector3Int[,] grid, Vector2Int start, Vector2Int end, int length)
    {
        TileNode endNode = null;
        TileNode startNode = null;

        var cols = tileNodes.GetUpperBound(0) + 1;
        var rows = tileNodes.GetUpperBound(1) + 1;

        // 타일의 에이스타정보(F,G,H, 부모 노드 등) 초기화 및 시작점과 끝 점 설정
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                tileNodes[i, j].Reset();
                if (tileNodes[i, j].X == start.x && tileNodes[i, j].Y == start.y)
                {
                    startNode = tileNodes[i, j];
                }
                else if (tileNodes[i, j].X == end.x && tileNodes[i, j].Y == end.y)
                    endNode = tileNodes[i, j];
            }
        }

        // 해당 경로가 유효하지 않으면 리턴
        if (!IsValidPath(grid, startNode, endNode))
        {
            // Debug.Log("InvalidPath");
            return null;
        }
        List<TileNode> openList = new List<TileNode>();
        List<TileNode> closedList = new List<TileNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 오픈리스트 중에서 F값이 가장 낮은 노드를 선택한다.
            int shorter = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].F < openList[shorter].F)
                    shorter = i;
                else if (openList[i].F == openList[shorter].F)
                {
                    if (openList[i].H < openList[shorter].H)
                        shorter = i;
                }
            }

            var current = openList[shorter];

            // 해당 노드가 끝점이라면 리턴
            if (endNode != null && openList[shorter] == endNode)
            {
                List<TileNode> path = new List<TileNode>();
                var temp = current;
                path.Add(temp);
                while (temp.previous != null)
                {
                    path.Add(temp.previous);
                    temp = temp.previous;
                }
                // 경로의 길이가 최대 개수를 초과하면 그만큼 뒤에서 삭제한다.
                if (length - (path.Count - 1) < 0)
                    path.RemoveRange(0, (path.Count - 1) - length);

                return path;
            }
            openList.Remove(current);
            closedList.Add(current);

            var neighbors = current.Neighbors;
            // 현재 노드의 이웃 노드들의 에이스타정보(F,G,H, 부모 노드 등)를 계산한다.
            for (int i = 0; i < neighbors.Count; i++)
            {
                var n = neighbors[i];
                if (n.Height == 1)
                    continue;

                // 대각선을 무조건 혀용하면 벽 모서리를 뚫고 지나갈 수 있기 때문에
                // 갈 수 있는 대각선인지 검사한다.
                if (!IsValidDiagonal(current.col, current.row, n.col, n.row))
                {
                    continue;
                }

                // 이웃 노드중에 끝점이 있다면 경로를 만들어 리턴한다.
                if (endNode != null && n == endNode)
                {
                    List<TileNode> path = new List<TileNode>();
                    n.previous = current;
                    var temp = n;
                    path.Add(temp);
                    while (temp.previous != null)
                    {
                        path.Add(temp.previous);
                        temp = temp.previous;
                    }
                    if (length - (path.Count - 1) < 0)
                        path.RemoveRange(0, (path.Count - 1) - length);
                    return path;
                }

                // 유효한 노드(갈 수 있는 노드)라면 정보 계산
                if (!closedList.Contains(n) && n.Height < 1)
                {
                    var tempG = current.G + 1;

                    bool newPath = false;

                    // 새로 갱신한 정보가 전에 있던 정보보다 더 빠른 길이라면 업데이트 한다.
                    if (openList.Contains(n))
                    {
                        if (tempG < n.G)
                        {
                            n.G = tempG;
                            newPath = true;
                        }
                    }
                    // 처음 방문한 노드
                    else
                    {
                        n.G = tempG;
                        newPath = true;
                        openList.Add(n);
                    }

                    if (newPath)
                    {
                        n.H = Heuristic(n, endNode);
                        n.F = n.G + n.H;
                        n.previous = current;
                    }
                }
            }
        }
        return null;
    }

    public int Heuristic(TileNode a, TileNode b)
    {
        //var dx = Math.Abs(a.X - b.Y);
        //var dy = Math.Abs(a.Y - b.Y);
        //return 1 * (dx + dy);

        #region diagonal

        //var d = 1;
        //var d2 = 1;

        var dx = Math.Abs(a.X - b.X);
        var dy = Math.Abs(a.Y - b.Y);

        //유클리드
        //return (int)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

        //맨해튼
        return (int)Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        #endregion
    }

    // 프로퍼티 정의부분
    public TileNode[,] TileNodes
    {
        get { return tileNodes; }
    }

    public Thread VectorFieldLoader
    {
        get { return vectorFieldLoader; }
    }

    public GameObject Indicator
    {
        set { indicator = value; }
    }

    public bool DebuggingText
    {
        set { debuggingText = value; }
    }

    public bool DebuggingIndicator
    {
        set { debuggingIndicator = value; }
    }

    public bool IsLoadedVectorField
    {
        get { return isLoadedVectorField; }
    }
}

public class TileNode
{
    // 유니티 타일맵 위치를 내부 배열 위치로 변환된 Vector2int 좌표
    public int col, row;

    // 유니티 타일맵 상에서의 Vector3Int좌표
    public int X, Y;

    // Astar용 변수
    public int F;
    public int G;
    public int H;

    // Astar사용 시 경로를 이어갈 때 사용할 부모 노드
    public TileNode previous = null;

    // 현재 타일이 이동 가능한 지형인지 여부(0이면 이동 가능 1이상이면 이동 불가)
    public int Height = 0;

    // 인접한 타일들의 리스트
    public List<TileNode> Neighbors;

    // 해당 타일에서 목적지를 Key값에 넣으면 Value로 해당 타일로 가는 방향이 나온다.
    public Dictionary<Vector2Int, Vector3> VectorField;

    // 벡터필드를 세팅할 때 기준 타일에서 이 타일까지 가는데 거쳐가는 타일의 갯수
    public int steps = 99999;

    // 해당 타일의 월드 상에서의 좌표
    public Vector3 worldPosition;

    // 현재 점유됐는지 여부
    public bool occupied = false;

    // 타일 점유자(건물 등의 갈 수 있는 지형의 유무가 동적으로 바뀌는 오브젝트들)
    public Character occupier = null;

    // 현재 타일이 어느 진영에 속해 있는지
    public Camp camp = Camp.End;

    public void Occupy(Character character, int height, bool occupy)
    {
        occupier = character;
        Height = height;
        occupied = occupy;
    }

    public TileNode(int x, int y, int height)
    {
        X = x;
        Y = y;
        F = G = H = 0;
        Neighbors = new List<TileNode>();
        Height = height;
        VectorField = new Dictionary<Vector2Int, Vector3>();
    }

    public void AddNeighbors(TileNode[,] grid, int x, int y)
    {
        if (x < grid.GetUpperBound(0))
            Neighbors.Add(grid[x + 1, y]);
        if (x > 0)
            Neighbors.Add(grid[x - 1, y]);
        if (y < grid.GetUpperBound(1))
            Neighbors.Add(grid[x, y + 1]);
        if (y > 0)
            Neighbors.Add(grid[x, y - 1]);

        // 상하좌우
        if (x < grid.GetUpperBound(0) && y > 0)
            Neighbors.Add(grid[x + 1, y - 1]);
        if (y < grid.GetUpperBound(1) && x > 0)
            Neighbors.Add(grid[x - 1, y + 1]);
        if (x > 0 && y > 0)
            Neighbors.Add(grid[x - 1, y - 1]);
        if (x < grid.GetUpperBound(0) && y < grid.GetUpperBound(1))
        {
            Neighbors.Add(grid[x + 1, y + 1]);
        }
    }

    // BFS로 이 타일로부터 모든 타일노드의 발걸음 정보를 세팅한다.
    public void SetSteps()
    {
        steps = 0;
        Queue<TileNode> queue = new Queue<TileNode>();

        for (int i = 0; i < Neighbors.Count; i++)
        {
            if (Neighbors[i].Height == 0 && Neighbors[i].steps > steps + 1)
            {
                if (!TilemapSystem.Instance.PathFinder.IsValidDiagonal(col, row, Neighbors[i].col, Neighbors[i].row))
                    continue;
                Neighbors[i].steps = steps + 1;
                queue.Enqueue(Neighbors[i]);
            }
        }
        // BFS
        while (queue.Count > 0)
        {
            TileNode tile = queue.Dequeue();
            for (int i = 0; i < tile.Neighbors.Count; i++)
            {
                // 벽의 모서리를 뚫는 대각선으로는 갈 수 없어야 하기 때문에 해당 타일을 걸러낸다.
                if (!TilemapSystem.Instance.PathFinder.IsValidDiagonal(tile.col, tile.row, tile.Neighbors[i].col, tile.Neighbors[i].row))
                    continue;
                // 갈 수 있는 지형이고 기존의 발걸음수가 현재 타일을 통해 가는 발걸음 수 보다 크면 갱신해준다.
                if (tile.Neighbors[i].steps > tile.steps + 1 && tile.Neighbors[i].Height.Equals(0))
                {
                    tile.Neighbors[i].steps = tile.steps + 1;
                    queue.Enqueue(tile.Neighbors[i]);
                }
            }
        }
    }

    public void Reset()
    {
        previous = null;
        F = G = H = 0;
    }
}