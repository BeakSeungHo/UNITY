using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookDir { Up = 0, Down, Left, Right };

/// <summary>
/// 
/// 최초 작성자 : 김영현 (20-10-19)
/// 
/// 최초 인게임 들어왔을 때 초기화.
/// 
/// 타일 마크 위치 표시
/// 
/// </summary>
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance = null;

    public GameObject CommanderPrefab = null;
    public InGameCamera MainCamera = null;
    // Start is called before the first frame update

    private Camp playerCamp = Camp.End;
    private Vector2Int curPlayerCommanderTile = Vector2Int.zero;
    public FogOfWar FogOfWar = null;
    public Dictionary<Camp, Commander> Commanders = new Dictionary<Camp, Commander>();

    public Dictionary<Camp, float> HungerTimeCount = new Dictionary<Camp, float>();
    public float HungerTimeMax = 60f;
    public int CommanderLevel = 0;

    /// <summary>
    /// 유닛 -> 빌딩 -> 커맨더 순서로 찾고, 타겟을 찾으면 다음 순서 순회 안함.
    /// </summary>
    /// <param name="from">탐색 중심 위치</param>
    /// <param name="range">탐색 범위</param>
    /// <param name="exceptCamp">탐색에서 제외할 캠프</param>
    /// <param name="attackType">공격 타입</param>
    /// <param name="foundObject">찾은 오브젝트</param>
    /// <returns></returns>
    public float Find_TargetInRange_ExceptCamp(Vector3 from, int range, Camp exceptCamp, AttackType attackType, out GameObject foundObject)
    {
        foundObject = null;
        float minDist = -1f;

        //  유닛 탐색
        var squads = SquadController.Instance.Squads;
        foreach (var squadPair in squads)
        {
            if (squadPair.Key == exceptCamp)
                continue;

            var squadList = squadPair.Value;

            //  분대 안 순회
            foreach (var squad in squadList)
            {
                //  유닛 순회
                foreach (var unit in squad.UnitList)
                {
                    //  타겟이 될 수 있는지 확인
                    if (!unit.CanBeTarget())
                        continue;

                    Vector3 to = unit.Pos;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                        continue;

                    //  공격 타입이랑 위치 체크.
                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != unit.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != unit.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundObject = unit.gameObject;
                            minDist = dist;
                        }
                    }
                }
            }
        }

        if (null != foundObject)
            return minDist;

        //  건물 탐색
        var buildings = BuildingManager.Instance.Buildings;
        foreach (var buildingPair in buildings)
        {
            //  같은 캠프 제외
            if (exceptCamp == buildingPair.Key)
                continue;

            // 빌딩 딕셔너리
            foreach (var buildingDic in buildingPair.Value)
            {
                //  빌딩 리스트
                var buildingList = buildingDic.Value;

                // 연결리스트의 각 노드(빌딩)
                var buildingNode = buildingList.First;

                //  빌딩 순회
                for (int i = 0; i < buildingList.Count; ++i)
                {
                    if (!buildingNode.Value.CanBeTarget())
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    Vector3 to = buildingNode.Value.transform.position;
                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundObject = buildingNode.Value.gameObject;
                            minDist = dist;
                        }
                    }
                    buildingNode = buildingNode.Next;
                }
            }
        }

        if (null != foundObject)
            return minDist;

        //  지휘관 탐색
        if (AttackType.Air != attackType)
        {
            foreach (var commanderPair in Commanders)
            {
                if (exceptCamp == commanderPair.Key)
                    continue;

                var commander = commanderPair.Value;

                Vector3 to = commander.Pos;

                //  시야가 밝혀진 곳인가?
                if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    continue;

                //  거리 계산
                float dist = Global.Calculate_TileDistance(from, to);

                if (dist <= range)
                {
                    if (minDist > dist || minDist < 0)
                    {
                        minDist = dist;
                        foundObject = commander.gameObject;
                    }
                }
            }
        }

        return minDist;
    }

    public float Find_TargetInRange_ForPig(Vector3 from, int range, Camp exceptCamp, AttackType attackType, out Character foundCharacter)
    {
        foundCharacter = null;
        float minDist = -1f;

        //  유닛 탐색
        var squads = SquadController.Instance.Squads;
        foreach (var squadPair in squads)
        {
            if (squadPair.Key == exceptCamp)
                continue;

            var squadList = squadPair.Value;

            //  분대 안 순회
            foreach (var squad in squadList)
            {
                //  유닛 순회
                foreach (var unit in squad.UnitList)
                {
                    //  타겟이 될 수 있는지 확인
                    if (!unit.CanBeTarget())
                        continue;

                    Vector3 to = unit.Pos;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                        continue;

                    //  공격 타입이랑 위치 체크.
                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != unit.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != unit.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundCharacter = unit;
                            minDist = dist;
                        }
                    }
                }
            }
        }

        //  지휘관 탐색
        if (AttackType.Air != attackType)
        {
            foreach (var commanderPair in Commanders)
            {
                if (exceptCamp == commanderPair.Key)
                    continue;

                var commander = commanderPair.Value;

                if (!commander.CanBeTarget())
                    continue;

                Vector3 to = commander.Pos;

                //  시야가 밝혀진 곳인가?
                if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    continue;

                //  거리 계산
                float dist = Global.Calculate_TileDistance(from, to);

                if (dist <= range)
                {
                    if (minDist > dist || minDist < 0)
                    {
                        minDist = dist;
                        foundCharacter = commander;
                    }
                }
            }
        }

        return minDist;
    }

    /// <summary>
    /// 유닛 -> 빌딩 -> 커맨더 순서로 찾고, 타겟을 찾으면 다음 순서 순회 안함.
    /// </summary>
    /// <param name="from">탐색 중심 위치</param>
    /// <param name="range">탐색 범위</param>
    /// <param name="exceptCamp">탐색에서 제외할 캠프</param>
    /// <param name="attackType">공격 타입</param>
    /// <param name="foundCharacter">찾은 캐릭터</param>
    /// <returns></returns>
    public float Find_TargetInRange_ExceptCamp(Vector3 from, int range, Camp exceptCamp, AttackType attackType, out Character foundCharacter)
    {
        foundCharacter = null;
        float minDist = -1f;

        //  유닛 탐색
        var squads = SquadController.Instance.Squads;
        foreach (var squadPair in squads)
        {
            if (squadPair.Key == exceptCamp)
                continue;

            var squadList = squadPair.Value;

            //  분대 안 순회
            foreach (var squad in squadList)
            {
                //  유닛 순회
                foreach (var unit in squad.UnitList)
                {
                    //  타겟이 될 수 있는지 확인
                    if (!unit.CanBeTarget())
                        continue;

                    Vector3 to = unit.Pos;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                        continue;

                    //  공격 타입이랑 위치 체크.
                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != unit.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != unit.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundCharacter = unit;
                            minDist = dist;
                        }
                    }
                }
            }
        }

        if (null != foundCharacter)
            return minDist;

        //  건물 탐색
        var buildings = BuildingManager.Instance.Buildings;
        foreach (var buildingPair in buildings)
        {
            //  같은 캠프 제외
            if (exceptCamp == buildingPair.Key)
                continue;

            // 빌딩 딕셔너리
            foreach (var buildingDic in buildingPair.Value)
            {
                //  빌딩 리스트
                var buildingList = buildingDic.Value;

                // 연결리스트의 각 노드(빌딩)
                var buildingNode = buildingList.First;

                //  빌딩 순회
                for (int i = 0; i < buildingList.Count; ++i)
                {
                    if (!buildingNode.Value.CanBeTarget())
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    Vector3 to = buildingNode.Value.transform.position;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundCharacter = buildingNode.Value;
                            minDist = dist;
                        }
                    }
                    buildingNode = buildingNode.Next;
                }
            }
        }

        if (null != foundCharacter)
            return minDist;

        //  지휘관 탐색
        if (AttackType.Air != attackType)
        {
            foreach (var commanderPair in Commanders)
            {
                if (exceptCamp == commanderPair.Key)
                    continue;

                var commander = commanderPair.Value;

                if (!commander.CanBeTarget())
                    continue;

                Vector3 to = commander.Pos;

                //  시야가 밝혀진 곳인가?
                if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    continue;

                //  거리 계산
                float dist = Global.Calculate_TileDistance(from, to);

                if (dist <= range)
                {
                    if (minDist > dist || minDist < 0)
                    {
                        minDist = dist;
                        foundCharacter = commander;
                    }
                }
            }
        }

        return minDist;
    }

    public float Find_TargetInRange_ForMineWire(Vector3 from, int range, Camp exceptCamp, AttackType attackType, out Character foundCharacter)
    {
        foundCharacter = null;
        float minDist = -1f;

        //  유닛 탐색
        var squads = SquadController.Instance.Squads;
        foreach (var squadPair in squads)
        {
            if (squadPair.Key == exceptCamp)
                continue;

            var squadList = squadPair.Value;

            //  분대 안 순회
            foreach (var squad in squadList)
            {
                //  유닛 순회
                foreach (var unit in squad.UnitList)
                {
                    //  타겟이 될 수 있는지 확인
                    if (!unit.CanBeTarget())
                        continue;

                    Vector3 to = unit.Pos;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                        continue;

                    //  공격 타입이랑 위치 체크.
                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != unit.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != unit.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundCharacter = unit;
                            minDist = dist;
                        }
                    }
                }
            }
        }

        if (null != foundCharacter)
            return minDist;

        //  건물 탐색
        var buildings = BuildingManager.Instance.Buildings;
        foreach (var buildingPair in buildings)
        {
            //  같은 캠프 제외
            if (exceptCamp == buildingPair.Key)
                continue;

            // 빌딩 딕셔너리
            foreach (var buildingDic in buildingPair.Value)
            {
                //  빌딩 리스트
                var buildingList = buildingDic.Value;

                // 연결리스트의 각 노드(빌딩)
                var buildingNode = buildingList.First;

                //  빌딩 순회
                for (int i = 0; i < buildingList.Count; ++i)
                {
                    if (!buildingNode.Value.CanBeTarget())
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    Vector3 to = buildingNode.Value.transform.position;

                    //  시야가 밝혀진 곳인가?
                    if (exceptCamp != Camp.End && !FogOfWar.Instance.CheckTileAlpha(to, exceptCamp))
                    {
                        buildingNode = buildingNode.Next;
                        continue;
                    }

                    switch (attackType)
                    {
                        case AttackType.Ground:
                            if (PlaceType.Ground != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                        case AttackType.Air:
                            if (PlaceType.Air != buildingNode.Value.Base.Data.PlaceType)
                                continue;
                            break;
                    }

                    //  거리 확인
                    float dist = Global.Calculate_TileDistance(from, to);

                    if (dist <= range)
                    {
                        if (minDist > dist || minDist < 0)
                        {
                            foundCharacter = buildingNode.Value;
                            minDist = dist;
                        }
                    }
                    buildingNode = buildingNode.Next;
                }
            }
        }

        if (null != foundCharacter)
            return minDist;
        return minDist;
    }

    public float Find_TargetInRange_ForWolfBuff(Vector3 from, int range, Camp includeCamp, AttackType attackType, out Character foundCharacter)
    {
        foundCharacter = null;
        float minDist = -1f;

        //  유닛 탐색
        var squads = SquadController.Instance.Squads;
        foreach (var squad in squads[includeCamp])
        {
            foreach (var unit in squad.UnitList)
            {
                //  타겟이 될 수 있는지 확인
                if (!unit.CanBeTarget() || !CanBeTargetForStim(unit.Base.Type))
                    continue;

                //   이미 버프 받았으면 다음으로
                if (unit.Stim)
                    continue;

                Vector3 to = unit.Pos;

                //  공격 타입이랑 위치 체크.
                switch (attackType)
                {
                    case AttackType.Ground:
                        if (PlaceType.Ground != unit.Base.Data.PlaceType)
                            continue;
                        break;
                    case AttackType.Air:
                        if (PlaceType.Air != unit.Base.Data.PlaceType)
                            continue;
                        break;
                }

                //  거리 확인
                float dist = Global.Calculate_TileDistance(from, to);

                if (dist <= range)
                {
                    if (minDist > dist || minDist < 0)
                    {
                        foundCharacter = unit;
                        minDist = dist;
                    }
                }
            }
        }

        //  건물 탐색
        var buildings = BuildingManager.Instance.Buildings;
        foreach (var buildingDic in buildings[includeCamp])
        {
            if (!CanBeTargetForStim(buildingDic.Key))
                continue;

            //  빌딩 리스트
            var buildingList = buildingDic.Value;

            // 연결리스트의 각 노드(빌딩)
            var buildingNode = buildingList.First;

            //  빌딩 순회
            for (int i = 0; i < buildingList.Count; ++i)
            {
                if (!buildingNode.Value.CanBeTarget())
                    continue;

                //   이미 버프 받았으면 다음으로
                if (buildingNode.Value.Stim)
                    continue;

                Vector3 to = buildingNode.Value.transform.position;

                switch (attackType)
                {
                    case AttackType.Ground:
                        if (PlaceType.Ground != buildingNode.Value.Base.Data.PlaceType)
                            continue;
                        break;
                    case AttackType.Air:
                        if (PlaceType.Air != buildingNode.Value.Base.Data.PlaceType)
                            continue;
                        break;
                }

                //  거리 확인
                float dist = Global.Calculate_TileDistance(from, to);

                if (dist <= range)
                {
                    if (minDist > dist || minDist < 0)
                    {
                        foundCharacter = buildingNode.Value;
                        minDist = dist;
                    }
                }
                buildingNode = buildingNode.Next;
            }
        }

        return minDist;
    }

    public Vector3Int Find_NearestEmptyTile(Vector3 worldPosition)
    {
        Vector3 respawnPos = Vector3.zero;

        var tilePos = TilemapSystem.Instance.WorldToCellPos(worldPosition);

        var searchTileList = StorageBoxes.Instance.CircleSearchList;

        for (int i = 0; i < StorageBoxes.Instance.CircleSearchMaxRange; ++i)
        {
            foreach (var addTile in searchTileList[i])
            {
                var checkTilePos = tilePos + addTile;

                //var worldPosition = TilemapSystem.Instance.CellToWorldPos(checkTilePos);
                var node = TilemapSystem.Instance.GetTile(worldPosition);

                if (null == node)
                    continue;

                if (TilemapSystem.Instance.IsWalkableTile(checkTilePos))
                {
                    return checkTilePos;
                }
            }
        }

        return Global.InvalidTilePos;
    }

    private bool CanBeTargetForStim(CommonType type)
    {
        switch (type)
        {
            case CommonType.Cabin:
            case CommonType.Mine:
            case CommonType.Wire:
            case CommonType.CampFire:
            case CommonType.Gristmill:
            case CommonType.Commander:
            case CommonType.Wolf:
                return false;
            default:
                return true;
        }
    }

    void Start()
    {
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CommanderLevel = 0;

        var CommanderList = GameManager.Instance.CommanderList;
        var UnitTypeDic = GameManager.Instance.UnitTypeDic;

        //  Commmander HP 초기화
        if (GameMode.Tutorial == GameManager.Instance.CurGameMode)
            SceneStarter.Instance.commonElements.CommonDataList[(int)CommonType.Commander].MaxHp = 500;
        else
            SceneStarter.Instance.commonElements.CommonDataList[(int)CommonType.Commander].MaxHp = 20;

        // 플레이어 세팅
        GameObject player = Instantiate(CommanderPrefab);
        playerCamp = CommanderList[0];

        var playerCommander = player.GetComponent<Commander>();
        playerCommander.Ready(playerCamp);
        Commanders.Add(playerCamp, playerCommander);

        //  플레이어 Squad 세팅
        SquadController.Instance.Ready_Squad(CommanderList[0], UnitTypeDic[CommanderList[0]]);
        MainCamera.Player = player;
        //AIManager.Instance.Commander = playerCommander;

        //  조이스틱 연결
        JoyStickCtrl.Instance.SetCommander(playerCommander);
        BattleUICtrl.Instance.SetCommander(playerCommander);

        //  대전모드 일 때
        if (GameMode.Tutorial != GameManager.Instance.CurGameMode &&
            GameMode.Campaign != GameManager.Instance.CurGameMode)
        {
            //  AI 세팅
            for (int i = 1; i < CommanderList.Count; ++i)
            {
                GameObject AI = Instantiate(CommanderPrefab);
                Commander AICommander = AI.GetComponent<Commander>();
                AICommander.Ready(CommanderList[i]);

                //  AI Squad 세팅
                SquadController.Instance.Ready_Squad(CommanderList[i], UnitTypeDic[CommanderList[i]]);

                // AI Manager와 AI를 연결
                AIManager.Instance.AI_Ready(CommanderList[i], AICommander);
                Commanders.Add(CommanderList[i], AICommander);
            }
        }

        //  캠페인 or 튜토리얼일 때
        else
            SquadController.Instance.Ready_Squad(Camp.Archimedes, UnitTypeDic[CommanderList[1]]);

        if (GameManager.Instance.CurGameMode != GameMode.Tutorial)
            TileMarking.Instance.MakeTileMarks();

        // 통계 초기화
        SceneStarter.Instance.statisticElements.GameStartStatistic();

        //SoundManager.Instance.Set_Volume(Sound_Channel.Ambient, 0.05f);

        if (!StorageBoxes.Instance.Ready_TileObjects())
            Debug.Log("Ready_TileObjects Failed.");
        else
            Debug.Log("Ready_TileOBjects success");

        //  BGM 
        SoundManager.Instance.Stop_BGM();
        var audios = SceneStarter.Instance.soundElements.BackSoundDic[playerCamp];
        SoundManager.Instance.Play_BGM(audios[1]);

        //  HungerCount 초기화
        HungerCount_Initialize();

        BattleUICtrl.Instance.Ready();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurGameMode != GameMode.Tutorial)
            Update_TileMark();

        //  식량 고갈 체크
        if (GameMode.Multi == GameManager.Instance.CurGameMode)
            HungerCheck();

        // Commander LevelUp
        if (GameMode.Multi == GameManager.Instance.CurGameMode)
            LevelUp_Commanders();

        //if (GameMode.Multi == GameManager.Instance.CurGameMode || GameMode.TimeAttack == GameManager.Instance.CurGameMode)
        //    Update_Test();
    }

    public void Surrender()
    {
        BuildingManager.Instance.DestroyAllBuilding(playerCamp);
    }

    public void LevelUp_Commanders()
    {
        if (CommanderLevel > 8)
            return;

        int playTime = (int)BattleUICtrl.Instance.PlayTime;

        int level = playTime / 60;
        //  커맨더 레벨업!
        if (level > CommanderLevel)
        {
            SceneStarter.Instance.commonElements.CommonDataList[(int)CommonType.Commander].MaxHp += 10;
            CommanderLevel = level;

            foreach (var commander in Commanders)
            {
                commander.Value.LevelUp();
            }
        }

    }

    private void HungerCheck()
    {
        foreach (var camp in GameManager.Instance.CommanderList)
        {
            //  식량 고갈됨.
            if (!BuildingManager.Instance.IsOnProductionFood(camp))
            {
                HungerTimeCount[camp] -= Time.deltaTime;

                if (HungerTimeCount[camp] <= 0f)
                {
                    BuildingManager.Instance.DestroyAllBuilding(camp);
                    break;
                }
            }
            //  고갈되지 않음.
            else
            {
                HungerTimeCount[camp] = HungerTimeMax;
            }
        }
    }

    private void HungerCount_Initialize()
    {
        foreach (var camp in GameManager.Instance.CommanderList)
        {
            if (!HungerTimeCount.ContainsKey(camp))
                HungerTimeCount.Add(camp, HungerTimeMax);
        }
    }

    private void Update_TileMark()
    {
        var commanders = GameManager.Instance.CommanderList;
        foreach (Camp commander in commanders)
        {
            if (!Commanders.ContainsKey(commander))
                continue;

            var commanderPos = Commanders[commander].transform.position;
            Vector3Int newTile = TilemapSystem.Instance.WorldToCellPos(commanderPos);

            //if (newTile != curPlayerCommanderTile)
            {
                //curPlayerCommanderTile = newTile;

                float dist = Mathf.Infinity;
                int dir = -1;

                if (TileMarking.Instance.curSize.ContainsKey(commander) == false)
                    return;

                if (TileMarking.Instance.curSize[commander] == 1)
                {
                    TileMarking.Instance.Set_TileMarkPos(commander, newTile, LookDir.Up);
                }
                else
                {
                    for (int i = 0; i < TileMarking.Instance.curSize[commander]; ++i)
                    {
                        var nearTile = TilemapSystem.Instance.CellToWorldPos(newTile + Global.LookDir[i]);

                        var nearTileNode = TilemapSystem.Instance.GetTile(nearTile);
                        if (null == nearTileNode)
                            continue;

                        var nearTileNodePos = nearTileNode.worldPosition;

                        float nearDist = (commanderPos - nearTileNodePos).magnitude;
                        if (dist > nearDist)
                        {
                            dir = i;
                            dist = nearDist;
                        }
                    }

                    if (-1 != dir)
                        TileMarking.Instance.Set_TileMarkPos(commander, newTile, (LookDir)dir);
                }
                bool check = TileMarking.Instance.CheckTileMark(commander);

                Color color = Color.red;
                //color.a = 0.35f;
                color.a = 0.25f;

                if (!check)
                {
                    TileMarking.Instance.SetColor(commander, color);
                }
                else
                {
                    color = Color.white;
                    //color.a = 0.35f;
                    color.a = 0.25f;
                    TileMarking.Instance.SetColor(commander, color);
                }
            }
        }
    }

    static int aiControllNumber = 0;
    public static Camp controllCamp = Camp.End;
    private void Update_Test()
    {
        if (Camp.End == controllCamp)
        {
            controllCamp = playerCamp;
        }

        var playerCommander = Commanders[playerCamp];
        var AICommander = Commanders[Camp.Archimedes];

        Vector3 moveDir = Vector3.zero;

        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.y = Input.GetAxis("Vertical");
        Commanders[controllCamp].Move_Commander(moveDir);

        if (Input.GetKeyDown(KeyCode.O))
        {
            --aiControllNumber;

            if (aiControllNumber < 0)
                aiControllNumber = 5;

            Commanders[controllCamp].Change_ControllSquad(aiControllNumber);
            var type = SquadController.Instance.SquadNumberToType[Commanders[controllCamp].Base.MyCamp][aiControllNumber];
            Debug.Log(controllCamp + " Select Squad : " + type);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ++aiControllNumber;

            if (aiControllNumber > 5)
                aiControllNumber = 0;

            AICommander.Change_ControllSquad(aiControllNumber);
            var type = SquadController.Instance.SquadNumberToType[AICommander.Base.MyCamp][aiControllNumber];
            Debug.Log(controllCamp + " Select Squad : " + type);
        }

        //if ()
        {
            Commanders[controllCamp].RallyAll_Commander(Input.GetKey(KeyCode.RightShift));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 100; i++)
            {
                GameObject pullUnitObj = PoolManager.Instance.PullObject(Pool_ObjType.Unit_Normal);
                CommonUnit unit = pullUnitObj.GetComponent<CommonUnit>();

                if (null == unit)
                    return;

                unit.Ready(Commanders[controllCamp].Base.MyCamp, Commanders[controllCamp].SelectedUnit, Commanders[controllCamp].transform.position);
                SquadController.Instance.Add_Unit(Commanders[controllCamp].Base.MyCamp, pullUnitObj);
            }
        }

        playerCommander.ReturnToBase(Input.GetKeyDown("."));

        if (Input.GetKeyDown("/"))
        {
            var audios = SceneStarter.Instance.soundElements.ComSoundDic[Camp.Bellafide][ComSoundType.Return];
            SoundManager.Instance.Play(Sound_Channel.Effect, playerCommander.gameObject, audios[1]);
        }
    }

}
