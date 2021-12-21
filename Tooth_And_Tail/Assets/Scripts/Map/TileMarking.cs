using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileMarking : MonoBehaviour
{
    public static TileMarking Instance = null;

    public GameObject TileMark = null;

    public Dictionary<Camp, List<GameObject>> TileMarks = new Dictionary<Camp, List<GameObject>>();

    private Dictionary<LookDir, List<Vector3Int>> dirToTilePos = null;

    public Dictionary<Camp, int> curSize = new Dictionary<Camp, int>();
    public Dictionary<Camp, bool> IsBuildable = new Dictionary<Camp, bool>();
    List<TileNode> PreNode = new List<TileNode>();

    public void Set_TileMarkPos(Vector3 position, LookDir dir)
    {
        TilemapSystem.Instance.WorldToCellPos(position);
    }

    public void Set_TileMarkPos(Camp camp, Vector3Int tilePos, LookDir dir)
    {
        if (curSize[camp] == 1 || GameManager.Instance.CurGameMode == GameMode.Campaign)
        {
            var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
            var node = TilemapSystem.Instance.GetTile(worldPos);
            if (null == node)
            {
                TileMarks[camp][0].SetActive(false);
                IsBuildable[camp] = false;
            }
            else
            {
                TileMarks[camp][0].SetActive(true);
                TileMarks[camp][0].transform.position = node.worldPosition;

                if (camp == GameManager.Instance.CommanderList[0])
                {
                    // 첫번째에 노드를 이전노드에 추가
                    if (PreNode.Count < 1)
                    {
                        PreNode.Add(node);
                    }
                    else
                    {
                        // 이전 노드의 occupier가 존재하면 UI OFF
                        if (PreNode[0] != node)
                        {
                            if (PreNode[0].occupier != null)
                            {
                                BuildingBase TempBuildingBase = (BuildingBase)PreNode[0].occupier;
                                TempBuildingBase.OffExtendSellUI();
                                TempBuildingBase.OffExtendBuildUI();
                            }
                            PreNode[0] = node;
                        }
                    }

                    if (node.occupier != null && GameManager.Instance.CurGameMode >= GameMode.Multi)
                    {
                        BuildingBase TempBuildingBase = (BuildingBase)node.occupier;
                        if (TempBuildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
                            TempBuildingBase.OnExtendSellUI();
                        else if (TempBuildingBase.Base.MyCamp == Camp.End)
                        {
                            TempBuildingBase.OnExtendBuildUI();
                            TempBuildingBase.extendSellUICtrl.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < curSize[camp]; ++i)
            {
                var markingTilePos = tilePos + dirToTilePos[dir][i];

                var worldPos = TilemapSystem.Instance.CellToWorldPos(markingTilePos);
                var node = TilemapSystem.Instance.GetTile(worldPos);

                if (null == node)
                {
                    TileMarks[camp][i].SetActive(false);
                    IsBuildable[camp] = false;
                }
                else
                {
                    TileMarks[camp][i].SetActive(true);
                    TileMarks[camp][i].transform.position = node.worldPosition;

                    if (camp == GameManager.Instance.CommanderList[0])
                    {
                        // 첫번째에 노드를 이전노드에 추가
                        if (PreNode.Count < 4)
                        {
                            PreNode.Add(node);
                        }
                        else
                        {
                            // 이전 노드의 occupier가 존재하면 UI OFF
                            if (PreNode[i] != node)
                            {
                                if (PreNode[i].occupier != null)
                                {
                                    BuildingBase TempBuildingBase = (BuildingBase)PreNode[i].occupier;
                                    TempBuildingBase.OffExtendSellUI();
                                    TempBuildingBase.OffExtendBuildUI();
                                }
                                PreNode[i] = node;
                            }
                        }

                        if (node.occupier != null && GameManager.Instance.CurGameMode >= GameMode.Multi)
                        {
                            BuildingBase TempBuildingBase = (BuildingBase)node.occupier;
                            if (TempBuildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
                                TempBuildingBase.OnExtendSellUI();
                            else if (TempBuildingBase.Base.MyCamp == Camp.End)
                            {
                                TempBuildingBase.OnExtendBuildUI();
                                TempBuildingBase.extendSellUICtrl.gameObject.SetActive(false);
                            }
                        }
                    }



                }
            }
        }
    }

    public void SetColor(Camp camp, Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            TileMarks[camp][i].GetComponent<SpriteRenderer>().color = color;
        }
    }

    public bool CheckTileMark(Camp camp)
    {
        // TileMark중 월드좌표의 Y축기준으로 맨 밑에 있는 타일에 건물을 건설하기 위해 해당 위치를 저장할 변수
        Vector3 LowestTile = Vector3.zero;

        IsBuildable[camp] = true;

        List<GameObject> tileMarks = TileMarking.Instance.TileMarks[camp];

        // 선택된 건물이 방어건물인 경우(한 칸만 차지하므로 따로 구분함)
        if (curSize[camp] == 1)
        {
            Vector2Int tilePos = TilemapSystem.Instance.WorldToTilePos(tileMarks[0].transform.position);
            if (TilemapSystem.Instance.IsBuildableTile(tileMarks[0].transform.position) == false)
                IsBuildable[camp] = false;
            var Type = SquadController.Instance.SquadNumberToType[camp][BattleUICtrl.Instance.CurUnitButtonIndex];
            if ((Type != CommonType.Mole && Type != CommonType.Mine) && TilemapSystem.Instance.GetTile(tileMarks[0].transform.position).camp != camp)
                IsBuildable[camp] = false;
        }
        else
        {
            List<Vector2Int> nodes = new List<Vector2Int>();

            float minY = 999999;

            for (int i = 0; i < tileMarks.Count; i++)
            {
                Vector2Int tilePos = TilemapSystem.Instance.WorldToTilePos(tileMarks[i].transform.position);
                Vector3 tileWorldPos = TilemapSystem.Instance.GetTile(tilePos).worldPosition;
                if (tileWorldPos.y < minY)
                {
                    LowestTile = tileWorldPos;
                    minY = tileWorldPos.y;
                }
                nodes.Add(tilePos);
            }
            // 타일마크가 차지하는 타일 중 하나라도 다른 건물이 차지하거나
            // 타일의 영역이 자신의 영역이 아니면 건설 불가능
            for (int i = 0; i < nodes.Count; i++)
            {
                if (TilemapSystem.Instance.IsBuildableTile(tileMarks[i].transform.position) == false)
                    IsBuildable[camp] = false;
                if (TilemapSystem.Instance.GetTile(tileMarks[i].transform.position).camp != camp)
                    IsBuildable[camp] = false;
            }
        }
        return IsBuildable[camp];
    }

    public void SetTileMark_Size(Camp camp, int size)
    {
        if (size == 4)
        {
            for (int i = 1; i < 4; i++)
            {
                TileMarks[camp][i].SetActive(true);
            }
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                TileMarks[camp][i].SetActive(false);
            }
        }
        curSize[camp] = size;
    }

    public void Set_TileMarkPos(Camp camp, List<TileNode> tileNodeList)
    {
        for (int i = 0; i < curSize[camp]; ++i)
        {
            if (null == tileNodeList[i])
            {
                TileMarks[camp][i].SetActive(false);
                IsBuildable[camp] = false;
            }
            else
            {
                TileMarks[camp][i].SetActive(true);
                TileMarks[camp][i].transform.position = tileNodeList[i].worldPosition;
            }
        }
    }

    public void Set_TileMarkPos(Camp camp, List<Vector3> positions)
    {
        for (int i = 0; i < curSize[camp]; ++i)
        {
            TileMarks[camp][i].transform.position = positions[i];
        }
    }

    public void TileMarkOn(Camp camp)
    {
        for (int i = 0; i < curSize[camp]; ++i)
            TileMarks[camp][i].SetActive(true);
    }

    public void TileMarkOff(Camp camp)
    {
        for (int i = 0; i < curSize[camp]; ++i)
            TileMarks[camp][i].SetActive(false);
    }

    public void MakeTileMarks()
    {
        var commanders = InGameManager.Instance.Commanders;

        foreach (var commander in commanders)
        {
            TileMarks.Add(commander.Value.camp, new List<GameObject>());
            curSize.Add(commander.Value.camp, 4);

            for (int j = 0; j < 4; ++j)
            {
                var mark = GameObject.Instantiate(TileMark, transform.position, Quaternion.identity, transform);
                if (GameManager.Instance.CurGameMode == GameMode.Campaign && j > 0)
                {
                    mark.SetActive(false);
                }
                if (commander.Value.Base.MyCamp != GameManager.Instance.CommanderList[0])
                {
                    mark.GetComponent<SpriteRenderer>().enabled = false;
                }
                TileMarks[commander.Value.camp].Add(mark);
            }
        }

    }

    // Start is called before the first frame update
    void Awake()
    {
        if (null != Instance)
        {
            Destroy(Instance);
        }
        Instance = this;

        //for (int i = 0; i < commanders.Count; i++)
        //{
        //    TileMarks.Add(commanders[i], new List<GameObject>());
        //    curSize.Add(commanders[i], 4);

        //    for (int j = 0; j < 4; ++j)
        //        TileMarks[commanders[i]].Add(GameObject.Instantiate(TileMark, transform.position, Quaternion.identity, transform));
        //}

        dirToTilePos = new Dictionary<LookDir, List<Vector3Int>>();

        dirToTilePos.Add(LookDir.Up, new List<Vector3Int>());
        dirToTilePos[LookDir.Up].Add(new Vector3Int(0, 0, 0));
        dirToTilePos[LookDir.Up].Add(new Vector3Int(1, 0, 0));
        dirToTilePos[LookDir.Up].Add(new Vector3Int(1, 1, 0));
        dirToTilePos[LookDir.Up].Add(new Vector3Int(0, 1, 0));

        dirToTilePos.Add(LookDir.Down, new List<Vector3Int>());
        dirToTilePos[LookDir.Down].Add(new Vector3Int(0, 0, 0));
        dirToTilePos[LookDir.Down].Add(new Vector3Int(-1, 0, 0));
        dirToTilePos[LookDir.Down].Add(new Vector3Int(-1, -1, 0));
        dirToTilePos[LookDir.Down].Add(new Vector3Int(0, -1, 0));

        dirToTilePos.Add(LookDir.Left, new List<Vector3Int>());
        dirToTilePos[LookDir.Left].Add(new Vector3Int(0, 0, 0));
        dirToTilePos[LookDir.Left].Add(new Vector3Int(-1, 0, 0));
        dirToTilePos[LookDir.Left].Add(new Vector3Int(-1, 1, 0));
        dirToTilePos[LookDir.Left].Add(new Vector3Int(0, 1, 0));

        dirToTilePos.Add(LookDir.Right, new List<Vector3Int>());
        dirToTilePos[LookDir.Right].Add(new Vector3Int(0, 0, 0));
        dirToTilePos[LookDir.Right].Add(new Vector3Int(1, 0, 0));
        dirToTilePos[LookDir.Right].Add(new Vector3Int(1, -1, 0));
        dirToTilePos[LookDir.Right].Add(new Vector3Int(0, -1, 0));
    }
}
