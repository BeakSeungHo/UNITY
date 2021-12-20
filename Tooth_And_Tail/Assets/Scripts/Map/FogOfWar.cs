using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FogOfWar : MonoBehaviour
{
    public static FogOfWar Instance = null;
    public Tilemap MinimapTilemap;
    public Tilemap tilemap;
    public BoundsInt tileBounds;
    public BuildingManager BuildingManager;
    int x;
    int y;
    public Camp tempFogCamp;
    //fog data
    private Dictionary<Camp, List<List<float>>> AlphaDictionary;
    private Dictionary<Camp, List<Vector3Int>> PreAlphaPosDictionary;
    private Dictionary<Camp, List<List<bool>>> PreAlphaVisitDictionary;
    List<int> PreAlphaCount;

    int tempX = 0;
    int tempY = 0;
    Color tempColor = new Color(0, 0, 0, 0);
    public Camera MainCamera;

    float lifeTime = 0f;

    List<bool> checkFlag;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        int row = 0;
        int col = 0;
        if (GameMode.Tutorial == GameManager.Instance.CurGameMode)
        {
            row = 50;
            col = 110;
        }
        if (GameMode.Multi == GameManager.Instance.CurGameMode || GameMode.Campaign == GameManager.Instance.CurGameMode || GameMode.TimeAttack == GameManager.Instance.CurGameMode)
        {
            row = 62;
            col = 62;
        }

        Instance = this;
        Instance.tileBounds = Instance.tilemap.cellBounds;

        Instance.PreAlphaPosDictionary = new Dictionary<Camp, List<Vector3Int>>(new CampComparer());

        Instance.PreAlphaCount = new List<int>();
        Instance.checkFlag = new List<bool>();
        //미리 할당
        for (int i = 0; i < GameManager.Instance.CommanderList.Count; i++)
        {
            List<Vector3Int> PreAlphaPosList = new List<Vector3Int>();
            List<int> PreAlphaSightList = new List<int>();
            for (int j = 0; j < 20; j++)
            {
                PreAlphaPosList.Add(new Vector3Int(0, 0, 0));
                PreAlphaSightList.Add(0);
            }
            Instance.PreAlphaCount.Add(0);
            Instance.PreAlphaPosDictionary.Add(GameManager.Instance.CommanderList[i], PreAlphaPosList);
            Instance.checkFlag.Add(false);
        }

        Instance.Ready(row, col);

        Instance.tempFogCamp = GameManager.Instance.CommanderList[0];

        Instance.BuildingManager = BuildingManager.Instance;
        Instance.lifeTime = 0f;

        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= 0.1f)
        {
            if (!InGameManager.Instance.MainCamera.EventFlag)
            {
                CheckVisit();

                for (int z = 0; z < GameManager.Instance.CommanderList.Count; z++)
                {
                    bool Multiflag = false;
                    // 커맨더
                    if (GameMode.Multi != GameManager.Instance.CurGameMode)
                    {
                        if (z > 0)
                            Multiflag = true;
                    }
                    if (!Multiflag)
                        LightFog(z, InGameManager.Instance.Commanders[GameManager.Instance.CommanderList[z]].transform.position, InGameManager.Instance.Commanders[GameManager.Instance.CommanderList[z]].Base.Sight,
                            GameManager.Instance.CommanderList[z]);
                    // 건물
                    var buildings = BuildingManager.Buildings[GameManager.Instance.CommanderList[z]];
                    foreach (var buildingList in buildings)
                    {
                        if (buildingList.Key == CommonType.Farm)
                            continue;
                        var buildingNode = buildingList.Value.First;

                        for (int i = 0; i < buildingList.Value.Count; i++)
                        {
                            LightFog(z, buildingNode.Value.transform.position, buildingNode.Value.Base.Sight, GameManager.Instance.CommanderList[z]);
                            buildingNode = buildingNode.Next;
                        }
                    }
                    // 유닛들
                    var Squads = SquadController.Instance.Squads[GameManager.Instance.CommanderList[z]];
                    for (int i = 0; i < Squads.Count; i++)
                    {
                        var UnitList = Squads[i].UnitList.First;

                        for (int j = 0; j < Squads[i].UnitList.Count; j++)
                        {
                            LightFog(z, UnitList.Value.transform.position, UnitList.Value.Base.Sight, GameManager.Instance.CommanderList[z]);
                            UnitList = UnitList.Next;
                        }
                    }
                }
            }
            ChangeTileColor(tempFogCamp);
            lifeTime = 0f;
        }
    }
    void CheckVisit()
    {
        for (int i = 0; i < GameManager.Instance.CommanderList.Count; i++)
        {
            if (!checkFlag[i])
            {
                if (PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]].Count != 0)
                {
                    for (int j = 0; j < PreAlphaCount[i]; j++)
                    {
                        AlphaDictionary[GameManager.Instance.CommanderList[i]][PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]][j].x + (tempX)][PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]][j].y + (tempY)] = 0.5f;
                        PreAlphaVisitDictionary[GameManager.Instance.CommanderList[i]][PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]][j].x + (tempX)][PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]][j].y + (tempY)] = false;
                        PreAlphaPosDictionary[GameManager.Instance.CommanderList[i]][j] = Vector3Int.zero;
                    }
                    PreAlphaCount[i] = 0;
                }
            }
        }
    }

    void LightFog(int fogCountIndex, Vector3 PivotPos, int Array, Camp fogCamp)
    {
        if (Array == 0)
            return;
        Vector3Int tempPos = tilemap.WorldToCell(PivotPos);
        TileType CommanderTileType = TilemapSystem.Instance.GetTileType(PivotPos);
        int CommanderTileFloor = TilemapSystem.Instance.GetTileElevation(ref tempPos);

        var circularList = StorageBoxes.Instance.CircleSearchList;

        for (int dist = 0; dist <= Array; ++dist)
        {
            for (int i = 0; i < circularList[dist].Count; i++)
            {
                Vector3Int lightingPos = tempPos + circularList[dist][i];
                int lightingFloor = TilemapSystem.Instance.GetTileElevation(ref lightingPos);

                if (CommanderTileType != TileType.Ramp && CommanderTileType != TileType.Wall)
                {
                    if (lightingFloor <= CommanderTileFloor)
                    {
                        ChangeAlphaData(lightingPos, fogCamp, true);
                        AddPreAlpha(fogCountIndex, fogCamp, lightingPos, Array);
                    }
                    // 플레이어 캠프일시 미니맵 밝힘
                    if (fogCamp == GameManager.Instance.CommanderList[0])
                    {
                        Color TempColor = MinimapTilemap.GetColor(lightingPos);
                        if (TempColor.a == 0)
                        {
                            MinimapTilemap.SetTileFlags(lightingPos, TileFlags.None);
                            TempColor.a = 1;
                            MinimapTilemap.SetColor(lightingPos, TempColor);
                        }
                    }
                }
                else
                {
                    if (lightingFloor <= CommanderTileFloor || lightingFloor == CommanderTileFloor + 1)
                    {
                        ChangeAlphaData(lightingPos, fogCamp, true);
                        AddPreAlpha(fogCountIndex, fogCamp, lightingPos, Array);
                        // 플레이어 캠프일시 미니맵 밝힘
                        if (fogCamp == GameManager.Instance.CommanderList[0])
                        {
                            Color TempColor = MinimapTilemap.GetColor(lightingPos);
                            if (TempColor.a == 0)
                            {
                                MinimapTilemap.SetTileFlags(lightingPos, TileFlags.None);
                                TempColor.a = 1;
                                MinimapTilemap.SetColor(lightingPos, TempColor);
                            }
                        }
                    }
                }
            }
        }
    }

    void ResetFog(Vector3 PivotPos, int Array, Camp fogCamp)
    {
        Vector3Int tempPos = tilemap.WorldToCell(PivotPos);
        TileType CommanderTileType = TilemapSystem.Instance.GetTileType(PivotPos);
        int CommanderTileFloor = TilemapSystem.Instance.GetTileElevation(ref tempPos);

        var circularList = StorageBoxes.Instance.CircleSearchList;

        for (int dist = 0; dist < Array; ++dist)
        {
            foreach (var addTile in circularList[dist])
            {
                var lightingPos = tempPos + addTile;
                int lightingFloor = TilemapSystem.Instance.GetTileElevation(ref lightingPos);

                if (CommanderTileType != TileType.Ramp && CommanderTileType != TileType.Wall)
                {
                    if (lightingFloor <= CommanderTileFloor)
                        ChangeAlphaData(lightingPos, fogCamp, false);
                }
                else
                {
                    if (lightingFloor <= CommanderTileFloor || lightingFloor == CommanderTileFloor + 1)
                    {
                        ChangeAlphaData(lightingPos, fogCamp, false);
                    }
                }
            }
        }
    }

    void ChangeAlphaData(Vector3Int tempPos, Camp fogCamp, bool flag)
    {
        int posX, posY;
        posX = tempPos.x + tempX;
        posY = tempPos.y + tempY;
        if ((posX >= 0 && posX <= x) && (posY >= 0 && posY <= y))
        {
            if (flag)
            {

                if (AlphaDictionary[fogCamp][posX][posY] > 0f)
                    AlphaDictionary[fogCamp][posX][posY] = 0f;
            }
            else
            {
                if (AlphaDictionary[fogCamp][posX][posY] < 0.5f)
                    AlphaDictionary[fogCamp][posX][posY] = 0.5f;
            }
        }
    }
    void ChangeTileColor(Camp fogCamp)
    {
        Vector3Int tempVec3Int = tilemap.WorldToCell(MainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, MainCamera.nearClipPlane)));
        for (int i = 15; i > -(15); i--)
        {
            for (int j = 15; j > -(15); j--)
            {
                Vector3Int tempPos = new Vector3Int(tempVec3Int.x + i, tempVec3Int.y + j, 0);
                int posX = tempPos.x + tempX;
                int posY = tempPos.y + tempY;
                if ((posX >= 0 && posX <= x) && (posY >= 0 && posY <= y))
                {
                    tempColor = new Color(1f, 1f, 1f, AlphaDictionary[fogCamp][posX][posY]);
                    if (tilemap.GetColor(tempPos).a != tempColor.a)
                    {
                        //tilemap.SetTileFlags(tempPos, TileFlags.None);

                        //타일 색 바꾸기
                        tilemap.SetColor(tempPos, (tempColor));
                    }
                }
            }
        }
    }
    void AlphaReset()
    {
        for (int x = tileBounds.xMin, i = 0; i < (tileBounds.size.x); x++, i++)
        {
            for (int y = tileBounds.yMin, j = 0; j < tileBounds.size.y; y++, j++)
            {

                Vector3Int v3Int = (new Vector3Int(x, y, 0));
                if (tilemap.HasTile(v3Int))
                {
                    tilemap.SetTileFlags(v3Int, TileFlags.None);
                    tempColor = new Color(1f, 1f, 1f, 0f);
                    tilemap.SetColor(v3Int, (tempColor));
                }
            }
        }
    }

    void Ready(int x, int y)
    {
        this.x = x;
        this.y = y;
        tempX = x / 2;
        tempY = y / 2;
        AlphaReset();
        AlphaDictionary = new Dictionary<Camp, List<List<float>>>(new CampComparer());
        PreAlphaVisitDictionary = new Dictionary<Camp, List<List<bool>>>(new CampComparer());
        for (int i = 0; i < GameManager.Instance.CommanderList.Count; i++)
        {
            List<List<float>> AlphaList = new List<List<float>>();
            List<List<bool>> VisitList = new List<List<bool>>();
            for (int j = 0; j <= x; j++)
            {
                List<float> Alpha = new List<float>();
                List<bool> Visit = new List<bool>();
                for (int z = 0; z <= y; z++)
                {
                    Alpha.Add(1f);
                    Visit.Add(false);
                }
                AlphaList.Add(Alpha);
                VisitList.Add(Visit);
            }
            AlphaDictionary.Add(GameManager.Instance.CommanderList[i], AlphaList);
            PreAlphaVisitDictionary.Add(GameManager.Instance.CommanderList[i], VisitList);
        }

        for (int i = tempX; i > -(tempX); i--)
        {
            for (int j = tempY; j > -(tempY); j--)
            {
                Vector3Int v3Int = (new Vector3Int(i, j, 0));

                tilemap.SetTileFlags(v3Int, TileFlags.None);
                tempColor = new Color(1f, 1f, 1f, 1f);
                tilemap.SetColor(v3Int, (tempColor));
            }
        }
    }

    public bool CheckTileAlpha(Vector3 TargetPos, Camp fogCamp)
    {
        Vector3Int tempPos = tilemap.WorldToCell(TargetPos);
        if (AlphaDictionary[fogCamp][tempPos.x + (x / 2)][tempPos.y + (y / 2)] > 0)
            return false;
        return true;
    }

    public void SetMinimapTilemap(Tilemap minimaptilemap)
    {
        MinimapTilemap = minimaptilemap;
    }

    void AddPreAlpha(int index, Camp camp, Vector3Int Pos, int Sight)
    {
        int posX, posY;
        posX = Pos.x + tempX;
        posY = Pos.y + tempY;
        if ((posX >= 0 && posX <= x) && (posY >= 0 && posY <= y))
        {
            if (!PreAlphaVisitDictionary[camp][posX][posY])
            {
                if (PreAlphaCount[index] < PreAlphaPosDictionary[camp].Count - 1)
                {
                    PreAlphaPosDictionary[camp][PreAlphaCount[index]] = Pos;
                    PreAlphaCount[index]++;

                }
                //초과시 새로 할당
                else
                {
                    PreAlphaPosDictionary[camp].Add(Pos);
                    PreAlphaCount[index]++;
                }
                PreAlphaVisitDictionary[camp][posX][posY] = true;
            }
        }
    }

    public void CheckSprite(Vector3 position, SpriteRenderer spriteRenderer)
    {
        if (CheckTileAlpha(position, tempFogCamp))
        {
            if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
            }
        }
        else
        {
            if (spriteRenderer.enabled)
            {
                spriteRenderer.enabled = false;
            }
        }
    }
    public void CheckSprite(Vector3 position, HPCanvas hpBar, SpriteRenderer spriteRenderer)
    {
        if (CheckTileAlpha(position, tempFogCamp))
        {
            if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
                hpBar.SetActiveUI(true);
            }
        }
        else
        {
            if (spriteRenderer.enabled)
            {
                spriteRenderer.enabled = false;
                hpBar.SetActiveUI(false);
            }
        }
    }
    public void CheckSprite(Vector3 position, HPCanvas hpBar, GameObject miniMap, SpriteRenderer spriteRenderer)
    {
        if (CheckTileAlpha(position, tempFogCamp))
        {
            if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
                miniMap.SetActive(true);
                hpBar.SetActiveUI(true);
            }
        }
        else
        {
            if (spriteRenderer.enabled)
            {
                spriteRenderer.enabled = false;
                miniMap.SetActive(false);
                hpBar.SetActiveUI(false);
            }
        }
    }
}
