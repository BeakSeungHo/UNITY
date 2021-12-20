using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileObjectInfo
{
    //public HitObject SkunkHitObject = null;
    public TileHitObject SkunkHitObject = null;
    public HashSet<Character> OccupiedUnitSet = new HashSet<Character>();
}

public class StorageBoxes : MonoBehaviour
{
    public static StorageBoxes Instance = null;

    //public LinkedList<HitObject>    BoxOfSkunkHit = null;
    public Dictionary<Vector3Int, TileObjectInfo> TileObjects = null;

    public GameObject testPrefab = null;
    public List<GameObject> VisualTestList = new List<GameObject>();

    public int CircleSearchMaxRange = 20;
    public List<List<Vector3Int>> CircleSearchList = new List<List<Vector3Int>>();

    public bool OccupiedVisualize = false;
    public bool Ready_TileObjects()
    {
        if (ReferenceEquals(TilemapSystem.Instance, null))
            return false;

        if (ReferenceEquals(TileObjects, null))
            TileObjects = new Dictionary<Vector3Int, TileObjectInfo>(new Vector3IntComparer());
        else
            TileObjects.Clear();

        //  테스트용 
        BoundsInt tileBounds = TilemapSystem.Instance.tileBounds;

        if (ReferenceEquals(tileBounds, null))
            return false;

        for (int x = tileBounds.xMin, i = 0; i < tileBounds.size.x; ++i, ++x)
        {
            for (int y = tileBounds.yMin, j = 0; j < tileBounds.size.y; ++j, ++y)
            {
                TileObjects.Add(new Vector3Int(x, y, 0), new TileObjectInfo());
            }
        }

        var xMax = tileBounds.xMin + tileBounds.size.x;
        var yMax = tileBounds.yMin + tileBounds.size.y;
        Debug.Log("tileObjects - x :" + tileBounds.xMin + " ~ " + xMax + " y : " + tileBounds.yMin + " ~ " + yMax);

        return true;
    }

    public void Ready_CircularList()
    {
        CircleSearchList.Capacity = CircleSearchMaxRange + 1;

        Queue<Vector3Int> bfs = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>(new Vector3IntComparer());

        bfs.Enqueue(Vector3Int.zero);
        visited.Add(Vector3Int.zero);

        while (bfs.Count > 0)
        {
            var tilePos = bfs.Dequeue();

            int dist = Mathf.RoundToInt(Vector3Int.Distance(tilePos, Vector3Int.zero));

            if (CircleSearchList.Count <= dist)
                CircleSearchList.Add(new List<Vector3Int>());

            CircleSearchList[dist].Add(tilePos);

            for (int i = 0; i < 4; ++i)
            {
                var nextTilePos = tilePos + new Vector3Int(Global.DirX[i], Global.DirY[i], 0);

                if (Mathf.RoundToInt(Vector3Int.Distance(nextTilePos, Vector3Int.zero)) > CircleSearchMaxRange)
                    continue;

                if (visited.Contains(nextTilePos))
                    continue;

                bfs.Enqueue(nextTilePos);
                visited.Add(nextTilePos);
            }
        }

        //for (int i = 1; i < CircleSearchMaxRange; ++i)
        //{
        //    var node = CircleSearchList[i].First;
        //    var zeroAnglePos = new Vector3(0f, 1f, 0f);
        //    for (int j = 0; j < CircleSearchList[i].Count - 1; ++j)
        //    {
        //        var checkNode = node;
        //        for (int k = j + 1; k < CircleSearchList[i].Count; ++k)
        //        {
        //            var nextNode = checkNode.Next;
        //            float angle = Vector3.Angle(zeroAnglePos, checkNode.Value);
        //            float nextAngle = Vector3.Angle(zeroAnglePos, nextNode.Value);

        //            if (angle > nextAngle)
        //            {
        //                var temp = checkNode.Value;
        //                checkNode.Value = nextNode.Value;
        //                nextNode.Value = temp;
        //            }

        //            checkNode = checkNode.Next;
        //        }
        //        node = node.Next;
        //    }
        //}
    }

    private void Awake()
    {
        Instance = this;

        //  테스트 온 오프
        OccupiedVisualize = false;

        //  원형 탐색을 위한 리스트 생성
        Ready_CircularList();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!OccupiedVisualize)
            return;

        int count = 0;
        foreach (var tileObjectInfo in TileObjects)
        {
            if (count == VisualTestList.Count)
            {
                for (int i = 0; i < 20; ++i)
                {
                    VisualTestList.Add(Instantiate(testPrefab));
                    VisualTestList[i].SetActive(false);
                }
            }

            if (tileObjectInfo.Value.OccupiedUnitSet.Count > 0)
            {
                VisualTestList[count].SetActive(true);

                var worldPos = TilemapSystem.Instance.CellToWorldPos(tileObjectInfo.Key);
                var node = TilemapSystem.Instance.GetTile(worldPos);

                VisualTestList[count].transform.position = node.worldPosition;
                ++count;
            }
        }
        for (int i = count; i < VisualTestList.Count; ++i)
            VisualTestList[i].SetActive(false);
    }
}
