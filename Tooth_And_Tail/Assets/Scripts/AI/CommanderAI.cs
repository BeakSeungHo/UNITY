using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


public class AI_GoalComparer : IEqualityComparer<CommanderAI.AI_Goal>
{
    bool IEqualityComparer<CommanderAI.AI_Goal>.Equals(CommanderAI.AI_Goal x, CommanderAI.AI_Goal y)
    {
        return x == y;
    }

    int IEqualityComparer<CommanderAI.AI_Goal>.GetHashCode(CommanderAI.AI_Goal obj)
    {
        return (int)obj;
    }
}

public class CommanderAI : MonoBehaviour
{
    public Commander commander = null;

    private CommanderFSM.STATE curState { get { return commander.commanderFSM.curState; } }

    public enum AI_Goal { Farming, Ready_War, Scout, Attack, Die, End }

    public AI_Goal curGoal = AI_Goal.Farming;
    public AI_Goal preGoal = AI_Goal.Farming;
    
    public Machine<CommanderAI> GoalMachine;
    public Dictionary<AI_Goal, FSM<CommanderAI>> AIGoalDic = new Dictionary<AI_Goal, FSM<CommanderAI>>(new AI_GoalComparer());


    //  AI의 입력값들. 
    public Vector3 moveDir = Vector3.zero;
    private Vector3 moveDestWorldPos = Vector3.zero;
    private TileNode curTile = null;
    private List<TileNode> path = null;
    private bool IsMoving = false;

    public bool diggingButton = false;
    
    public bool commandMoveAllButton = false;
    public bool commandMoveButton = false;

    public bool buildButton = false;

    //  Goal을 결정하기 위한 변수값들.
    public int Food = 0;
    public int UnitCount = 0;
    public int UnitBuildingCount = 0;
    public int FarmCount = 0;
    public int RemainedWorldGristmill = 0;
    public float PlayTime = 0f;
    public bool CanFarming = true;

    private Coroutine goalCor = null;
    
    public HashSet<Vector3Int> EnemyBuildingTile = new HashSet<Vector3Int>();

    private int BuildCost(CommonType type) { return SceneStarter.Instance.commonElements.CommonDataList[(int)type].BuildCost; }

    public Vector3 Pos { get { return commander.transform.position; } }
    public Camp MyCamp { get { return commander.Base.MyCamp; } }

    //public GameObject DestIndicatorPrefab = null;
    //public GameObject NextIndicatorPrefab = null;
    //public GameObject ArrowIndicatorPrefab = null;

    //private GameObject destIndicatorInst = null;
    //private GameObject nextIndicatorInst = null;
    //private GameObject arrowForNext = null;
    //private GameObject arrowForMoveDir = null;

    //public bool DebugOn = false;

    public CommanderAI()
    {
        GoalMachine = new Machine<CommanderAI>();

        AIGoalDic.Add(AI_Goal.Farming,      new CommanderAIFarming(this));
        AIGoalDic.Add(AI_Goal.Ready_War,    new CommanderAIReadyWar(this));
        AIGoalDic.Add(AI_Goal.Scout,        new CommanderAIScout(this));
        AIGoalDic.Add(AI_Goal.Attack,       new CommanderAIAttack(this));
        AIGoalDic.Add(AI_Goal.Die,          new CommanderAIDie(this));

        GoalMachine.SetState(AIGoalDic[AI_Goal.Farming], this);
    }
    
    ~CommanderAI()
    {
        if (!ReferenceEquals(null, goalCor))
            StopCoroutine(goalCor);
    }

    public void Reset()
    {
        moveDir = Vector3.zero;
        moveDestWorldPos = Vector3.zero;

        curTile = null;
        path = null;

        diggingButton = false;
        commandMoveAllButton = false;
        commandMoveButton = false;
        buildButton = false;
    }

    public void Ready(Commander commander)
    {
        this.commander = commander;

        curGoal = AI_Goal.Farming;

        //DebugOn = false;

        //if (DebugOn)
        //{
        //    if (null != DestIndicatorPrefab)
        //        destIndicatorInst = Instantiate(DestIndicatorPrefab, transform);
        //    if (null != NextIndicatorPrefab)
        //        nextIndicatorInst = Instantiate(NextIndicatorPrefab, transform);
        //    if (null != ArrowIndicatorPrefab)
        //    {
        //        arrowForMoveDir = Instantiate(ArrowIndicatorPrefab, transform);
        //        arrowForNext = Instantiate(ArrowIndicatorPrefab, transform);
        //    }
        //}
    }

    public void ChangeGoal(AI_Goal newGoal)
    {
        if (AIGoalDic.ContainsKey(newGoal))
            GoalMachine.Change(AIGoalDic[newGoal]);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Update()
    {
        if (ReferenceEquals(null, goalCor))
            goalCor = StartCoroutine("Setting_Goal");
        //Set_Goal();

        //if (DebugOn && null != nextIndicatorInst && null != arrowForNext)
        //{
        //    if (null != curTile)
        //    {
        //        nextIndicatorInst.SetActive(true);
        //        arrowForNext.SetActive(true);
        //        nextIndicatorInst.transform.position = curTile.worldPosition;

        //    }
        //    else
        //    {
        //        nextIndicatorInst.SetActive(false);
        //        arrowForNext.SetActive(false);
        //    }

        //    if (moveDir != Vector3.zero)
        //    {
        //        arrowForMoveDir.SetActive(true);

        //        Quaternion quaternion = Quaternion.FromToRotation(new Vector3(0f, 1f, 0f), moveDir);
        //        float angle = quaternion.eulerAngles.z;
        //        arrowForMoveDir.transform.eulerAngles = new Vector3(0f, 0f, angle);

        //        arrowForMoveDir.transform.position = Pos;
        //    }
        //    else
        //        arrowForMoveDir.SetActive(false);
        //}

        GoalMachine.Run();

        if (!IsMoving)
            moveDir = Vector3.zero;

        IsMoving = false;

        AI_ButtonClick();
    }

    public IEnumerator Setting_Goal()
    {
        while(true)
        {
            Refresh_GoalVariables();

            Set_Goal();
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 행동을 결정하기 위한 변수들을 
    /// </summary>
    public void Refresh_GoalVariables()
    {
        Food = GameManager.Instance.GetFood(MyCamp);
        UnitCount = Get_SquadUnitCount();

        UnitBuildingCount = Get_UnitBuildingCount();
        PlayTime = BattleUICtrl.Instance.PlayTime;

        FarmCount = Get_FarmCount();
        RemainedWorldGristmill = Get_RemainedWorldGristmill();

        Check_EnemyBuildingTile();
    }

    /// <summary>
    /// AI 행동목표를 설정한다.
    /// </summary>
    public void Set_Goal()
    {
        if (CommanderFSM.STATE.DEATH == commander.commanderFSM.curState)
        {
            ChangeGoal(AI_Goal.Die);
            return;
        }

        // 플레이 타임 1분 전에는 파밍 우선
        if (PlayTime < 60f)
        {
            //curGoal = AI_Goal.Farming;
            ChangeGoal(AI_Goal.Farming);
            return;
        }
        
        //  생산될 유닛보다 농장 갯수가 너무 적으면 파밍 우선
        if (FarmCount < UnitBuildingCount * 2)
        {
            if (CanFarming)
            {
                ChangeGoal(AI_Goal.Farming);
                return;
            }
        }

        //  빌딩 수가 너무 적거나 나온 유닛 수가 적으면 싸움 준비 우선
        if (UnitBuildingCount < 3 || UnitCount < 9)
        {
           // curGoal = AI_Goal.Ready_War;
            ChangeGoal(AI_Goal.Ready_War);
            return;
        }

        //  유닛 수가 충분히 많으면 정찰 우선
        if (UnitCount >= 9)
        {
            //  찾은 적 빌딩 위치가 없으면 정찰 우선
            if (EnemyBuildingTile.Count == 0)
            {
                //curGoal = AI_Goal.Scout;
                ChangeGoal(AI_Goal.Scout);
                return;
            }
            //  찾은 적 빌딩 위치가 있으면 공격 우선
            else
            {
                //curGoal = AI_Goal.Attack;
                ChangeGoal(AI_Goal.Attack);
                return;
            }
        }
    }

    /// <summary>
    /// 버튼 클릭 일괄 처리
    /// </summary>
    public void AI_ButtonClick()
    {
        commander.Move_Commander(moveDir);
        commander.Digging(diggingButton);

        if (buildButton)
        {
            commander.BuildBuilding();
            buildButton = false;
        }

        commander.Rally_Commander(commandMoveButton);
        commander.RallyAll_Commander(commandMoveAllButton);
    }

    /// <summary>
    /// 커맨더가 선택하고 있는 유닛을 랜덤으로 바꿈
    /// </summary>
    public void Change_SelectedUnitRandom()
    {
        int rangeMax = SquadController.Instance.Squads[commander.Base.MyCamp].Count - 1;
        commander.Change_ControllSquad(Random.Range(0, rangeMax));
    }

    /// <summary>
    /// 커맨더가 선택하고 있는 유닛을 플레이 타임에 따라 랜덤으로 바꿈
    /// </summary>
    public void Change_SelectedUnitRandom_ByPlayTime()
    {
        int rangeMax = SquadController.Instance.Squads[commander.Base.MyCamp].Count - 1;
        if (PlayTime < 120f)
            rangeMax = 1;
        else if (PlayTime < 180f)
            rangeMax = 3;

        commander.Change_ControllSquad(Random.Range(0, rangeMax));
    }
    
    /// <summary>
    /// 생산된 유닛 수를 센다.
    /// </summary>
    /// <returns>유닛 수</returns>
    public int Get_SquadUnitCount()
    {
        int unitCount = 0;

        var mySquads = SquadController.Instance.Squads[commander.Base.MyCamp];

        for (int i = 0; i < mySquads.Count; ++i)
        {
            unitCount += mySquads[i].UnitList.Count;
        }

        return unitCount;
    }

    /// <summary>
    /// 건물 수를 센다.
    /// </summary>
    /// <returns>건물 수</returns>
    public int Get_UnitBuildingCount()
    {
        var buildings = BuildingManager.Instance.Buildings[MyCamp];

        int buildingCount = 0;
        foreach (var pair in buildings)
        {
            if (CommonType.WarrenT1 <= pair.Key && pair.Key <= CommonType.WarrenT3)
            {
                buildingCount += pair.Value.Count;
            }
        }

        return buildingCount;
    }

    /// <summary>
    /// 생산중인 농장의 수를 센다.
    /// </summary>
    /// <returns></returns>
    public int Get_FarmCount()
    {
        var buildings = BuildingManager.Instance.Buildings[MyCamp];

        int farmCount = 0;

        foreach (var buildingBase in buildings[CommonType.Gristmill])
        {
            var gristmill = buildingBase.transform.GetComponent<Gristmill>();

            var farms = gristmill.Farms;

            for (int i = 0; i < 8; ++i)
            {
                if (farms[i].exhaust)
                    continue;
                if (Farm.FarmState.Production == farms[i].GetState())
                    ++farmCount;
            }
        }

        return farmCount;
    }

    /// <summary>
    /// 점령되지 않은 제분소를 센다.
    /// </summary>
    /// <returns>점령되지 않은 제분소 수</returns>
    public int Get_RemainedWorldGristmill()
    {
        int gristmillCount = 0;

        foreach(var gristmillPos in AIManager.Instance.GristmillPositions)
        {
            var node = TilemapSystem.Instance.GetTile(gristmillPos);

            if (null == node)
            {
                Debug.Log("Get_RemainedWorldGristmill error : gristmillPos Node is null");
                continue;
            }

            var occupier = node.occupier;

            if (null == occupier)
            {
                Debug.Log("Get_RemainedWorldGristmill error : occupier is null");
                continue;
            }

            if (CommonType.Gristmill != occupier.Base.Type)
            {
                Debug.Log("Get_RemainedWorldGristmill error : There is no gristmill in gristmillPos");
                continue;
            }

            if (Camp.End == occupier.Base.MyCamp)
                ++gristmillCount;
        }

        return gristmillCount;
    }

    /// <summary>
    /// 가까운 캠프파이어를 찾는다.
    /// </summary>
    /// <param name="campFirePosition">찾은 위치</param>
    /// <returns>있으면 true, 없으면 false</returns>
    public bool Check_CampFire(out Vector3 campFirePosition)
    {
        campFirePosition = Global.InvalidWorldPos;

        var commanderPos = commander.transform.position;

        float minDist = -1f;

        if (!BuildingManager.Instance.Buildings.ContainsKey(Camp.End))
        {
            Debug.Log("buildings has not " + Camp.End + " key");
            return false;
        }

        if (!BuildingManager.Instance.Buildings[Camp.End].ContainsKey(CommonType.CampFire))
        {
            Debug.Log("buildings[" + Camp.End + "] has not " + CommonType.CampFire + " key");
            return false;
        }

        var campFires = BuildingManager.Instance.Buildings[Camp.End][CommonType.CampFire];

        foreach (var campFire in campFires)
        {
            if (!campFire.activeSelf)
                continue;

            var campFirePos = campFire.transform.position;
            int dist = Global.Calculate_TileDistance(campFirePos, commanderPos);

            if (minDist < 0f || minDist > dist)
            {
                minDist = dist;
                campFirePosition = campFirePos;
            }
        }

        return minDist >= 0f;
    }

    /// <summary>
    /// 가까운 카빈을 찾는다.
    /// </summary>
    /// <param name="cabinPosition">찾은 위치</param>
    /// <returns>있으면 true, 없으면 false</returns>
    public bool Check_Cabin(out Vector3 cabinPosition)
    {
        cabinPosition = Global.InvalidWorldPos;

        var commanderPos = commander.transform.position;

        float minDist = -1f;

        if (!BuildingManager.Instance.Buildings.ContainsKey(Camp.End))
        {
            Debug.Log("buildings has not " + Camp.End + " key");
            return false;
        }

        if (!BuildingManager.Instance.Buildings[Camp.End].ContainsKey(CommonType.Cabin))
        {
            Debug.Log("buildings[" + Camp.End + "] has not " + CommonType.Cabin + " key");
            return false;
        }

        var cabins = BuildingManager.Instance.Buildings[Camp.End][CommonType.Cabin];
        
        foreach( var cabin in cabins)
        {
            if (!cabin.activeSelf)
                continue;

            var cabinPos = cabin.transform.position;
            int dist = Global.Calculate_TileDistance(cabinPos, commanderPos);

            if (minDist < 0f || minDist > dist)
            {
                minDist = dist;
                cabinPosition = cabinPos;
            }

        }

        return minDist >= 0f;
    }

    public bool Scout_EnemyBuilding()
    {
        var circularList = StorageBoxes.Instance.CircleSearchList;
        var range = commander.Base.Sight;

        var commanderTilePos = TilemapSystem.Instance.WorldToCellPos(Pos);

        for (int i = 0; i <= range; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var checkTilePos = commanderTilePos + addTile;
                var checkWorldPos = TilemapSystem.Instance.CellToWorldPos(checkTilePos);

                var node = TilemapSystem.Instance.GetTile(checkWorldPos);

                if (null == node)
                    continue;

                var occupier = node.occupier;
                if (null == occupier)
                    continue;

                if (occupier.Base.MyCamp == commander.Base.MyCamp || 
                    Camp.End == occupier.Base.MyCamp)
                    continue;

                if (CommonType.Gristmill != occupier.Base.Type && 
                    CommonType.Farm != occupier.Base.Type)
                    continue;

                EnemyBuildingTile.Add(checkTilePos);
                return true;
            }
        }


        return false;
    }

    /// <summary>
    /// 적 건물 위치에 여전히 적 빌딩이 있는지 확인하여 제거함.
    /// </summary>
    public void Check_EnemyBuildingTile()
    {
        foreach (var tilePos in EnemyBuildingTile)
        {
            var checkWorldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
            var checkNode = TilemapSystem.Instance.GetTile(checkWorldPos);

            if (null == checkNode?.occupier)
            {
                EnemyBuildingTile.Remove(tilePos);
                return;
            }

            var occupier = checkNode.occupier;

            if (occupier.Base.MyCamp == commander.Base.MyCamp ||
                Camp.End == occupier.Base.MyCamp ||
                (CommonType.Gristmill != occupier.Base.Type &&
                CommonType.Farm != occupier.Base.Type))
            {
                EnemyBuildingTile.Remove(tilePos);
                return;
            }
        }
    }

    /// <summary>
    /// 제분소 위치로 이동
    /// </summary>
    /// <returns></returns>
    public bool Find_PathToGristmills()
    {
        //var buildings = BuildingManager.Instance.Buildings[]
        var gristmillList = AIManager.Instance.GristmillPositions;

        int infintyLoop = 0;
        while (true)
        {
            ++infintyLoop;

            if (infintyLoop > 100)
            {
                Debug.Log("Find_PathToGristmills : infinityLoop");
                break;
            }

            int randomIndex = Random.Range(0, gristmillList.Count - 1);

            var node = TilemapSystem.Instance.GetTile(gristmillList[randomIndex]);

            if (null == node.occupier ||
                commander.Base.MyCamp == node.occupier.Base.MyCamp)
                continue;

            var destTile = InGameManager.Instance.Find_NearestEmptyTile(gristmillList[randomIndex]);

            if (destTile != Global.InvalidTilePos)
            {
                return Find_Path(destTile);
            }
        }

        return false;
    }

    public bool Find_RandomScoutPath()
    {
        var tilebounds = TilemapSystem.Instance.tileBounds;
        Vector2Int gridMax = new Vector2Int(tilebounds.size.x, tilebounds.size.y);
        Vector2Int gridMin = Vector2Int.zero;

        Vector3Int tileMax = TilemapSystem.Instance.GridPosToCellPos(gridMax);
        Vector3Int tileMin = TilemapSystem.Instance.GridPosToCellPos(gridMin);

        int randX = (int)Random.Range(tileMin.x, tileMax.x);
        int randY = (int)Random.Range(tileMin.y, tileMax.y);
        Vector3Int randTilePos = new Vector3Int(randX, randY, 0);

        var circularList = StorageBoxes.Instance.CircleSearchList;
        var circularListMaxRange = StorageBoxes.Instance.CircleSearchMaxRange;
        for (int i = 0; i < circularListMaxRange; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var tilePos = randTilePos + addTile;

                if (!Find_Path(tilePos))
                    continue;
                else
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 건물을 짓기위한 월드 위치를 가져온다.
    /// </summary>
    /// <returns></returns>
    public Vector3 Get_WorldPosForBuild()
    {
        var gristmills = BuildingManager.Instance.Buildings[commander.Base.MyCamp][CommonType.Gristmill];
        var circularList = StorageBoxes.Instance.CircleSearchList;

        foreach (var building in gristmills)
        {
            var gristmillTilePos = TilemapSystem.Instance.WorldToCellPos(building.transform.position);

            for (int i = 0; i < 12; ++i)
            {
                foreach (var addTile in circularList[i])
                {
                    var tilePos = gristmillTilePos + addTile;

                    bool tile4Check = true;

                    Vector3 worldCheckPos = Vector3.zero;
                    for (int j = 0; j < 4; ++j)
                    {
                        var checkTilePos = tilePos + Global.UpTiles[j];

                        if (!TilemapSystem.Instance.IsBuildableTile(checkTilePos))
                        {
                            tile4Check = false;
                            break;
                        }

                        worldCheckPos += TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(checkTilePos)).worldPosition;
                    }

                    if (!tile4Check)
                        continue;

                    worldCheckPos /= 4;
                    return worldCheckPos;
                }
            }
        }

        return Global.InvalidWorldPos;
    }

    /// <summary>
    /// 건설중인 제분소가 있는지 체크
    /// </summary>
    /// <returns>있으면 true, 없으면 false</returns>
    public bool Check_GristmillUnderConstruction()
    {
        var gristmills = BuildingManager.Instance.Buildings[commander.Base.MyCamp][CommonType.Gristmill];

        foreach(var building in gristmills)
        {
            var gristmill = building.GetComponent<Gristmill>();

            if (null == gristmill)
                continue;

            if (BuildingState.Construct == gristmill.buildingBase.GetBuildingStateOperator().CurBuildingState)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 농장을 지을 타일 위치를 가져온다.
    /// </summary>
    /// <returns></returns>
    public Vector3Int Get_FirstFarmTilePos()
    {
        var gristmills = BuildingManager.Instance.Buildings[commander.Base.MyCamp][CommonType.Gristmill];

        foreach (var building in gristmills)
        {
            var gristmill = building.GetComponent<Gristmill>();

            if (null == gristmill)
                continue;

            for (int j = 0; j < 8; ++j)
            {
                var farm = gristmill.Farms[j];

                var farmState = farm.GetState();

                if (farm.exhaust)
                    continue;

                if (Farm.FarmState.Idle == farmState)
                {
                    return TilemapSystem.Instance.WorldToCellPos(farm.transform.position);
                }
            }
        }

        return Global.InvalidTilePos;
    }

    /// <summary>
    /// 제분소를 짓기 위한 위치 찾기
    /// </summary>
    /// <returns></returns>
    public Vector3 Get_WorldPosForBuildGristmill()
    {
        var curTilePos = TilemapSystem.Instance.WorldToCellPos(Pos);
        var gristmillPositions = AIManager.Instance.GristmillPositions;

        Vector3 closestPos = Global.InvalidWorldPos;

        int minPathCount = -1;

        foreach (var worldPos in gristmillPositions)
        {
            var node = TilemapSystem.Instance.GetTile(worldPos);

            if (null == node.occupier)
                continue;

            if (Camp.End == node.occupier.Base.MyCamp)
            {
                var tilePos = InGameManager.Instance.Find_NearestEmptyTile(worldPos);
                var nearWorldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);

                //  현재 있는 위치와 찾는 위치가 같음.
                if (curTilePos == TilemapSystem.Instance.WorldToCellPos(nearWorldPos))
                {
                    minPathCount = 0;
                    closestPos = worldPos;
                    break;
                }

                var path = TilemapSystem.Instance.GetPath(Pos, nearWorldPos);

                if (null == path)
                    continue;

                if (minPathCount < 0 || path.Count < minPathCount)
                {
                    minPathCount = path.Count;
                    closestPos = worldPos;
                }
            }
        }

        return closestPos;
    }

    /// <summary>
    /// 시야범위 만큼 순회하며 적 건물이 있는지 확인하여 건물 위치를 저장한다.
    /// </summary>
    /// <returns>true 는 찾음, false 는 못찾음</returns>
    public bool Check_EnemyBuilding()
    {
        var circularList = StorageBoxes.Instance.CircleSearchList;
        var range = commander.Base.Sight;

        var curTilePos = TilemapSystem.Instance.WorldToCellPos(commander.Pos);

        for (int i = 0; i < range; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var tilePos = curTilePos + addTile;

                var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));
                if (null == node)
                    continue;

                var building = node.occupier;
                if (!ReferenceEquals(null, building))
                {
                    //if (CommonType.Gristmill != building.Base.Type)
                    //    continue;

                    Camp buildingCamp = building.Base.MyCamp;
                    
                    if (commander.Base.MyCamp != buildingCamp &&
                        Camp.End != buildingCamp)
                    {
                        EnemyBuildingTile.Add(tilePos);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 저장해 놓은 적 빌딩중에 유효한 건물이 있는지 확인하고 그 위치가 커맨더 시야 안에 있는지 확인
    /// </summary>
    /// <returns>시야 안에 있으면 true, 유효한 건물이 없거나, 시야 안에 없으면 false</returns>
    public bool Check_EnemyBuildingTile_InRange()
    {
        if (0 == EnemyBuildingTile.Count)
            return false;

        var curTilePos = TilemapSystem.Instance.WorldToCellPos(Pos);

        foreach(var tilePos in EnemyBuildingTile)
        {
            var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);

            var node = TilemapSystem.Instance.GetTile(worldPos);

            if (null == node)
            {
                Debug.Log("CommanderAI : tilePos in EnemyBuildingTile is invalid");
                EnemyBuildingTile.Remove(tilePos);
                continue;
            }

            if (null == node.occupier)
            {
                Debug.Log("CommanderAI : tilePos in EnemyBuildingTile is invalid");
                EnemyBuildingTile.Remove(tilePos);
                continue;
            }

            if (Vector3Int.Distance(curTilePos, tilePos) < commander.Base.Sight)
                return true;
        }

        return false;
    }

    public bool Check_AllUnitState_Idle()
    {
        foreach (var Squad in SquadController.Instance.Squads[MyCamp])
        {
            foreach (var unit in Squad.UnitList)
            {
                if (!unit.IsIdle())
                    return false;
            }
        }

        return true;
    }

    public bool Check_AllUnitState_NotIdle()
    {
        foreach (var Squad in SquadController.Instance.Squads[MyCamp])
        {
            foreach (var unit in Squad.UnitList)
            {
                if (unit.IsIdle())
                    return false;
            }
        }

        return true;
    }

    public bool IsArrive(Vector3 from, Vector3 To)
    {
        var fromTo = To - from;
        fromTo.z = 0f;

        if (fromTo.magnitude < 0.1f)
            return true;
        else
        {
            //if (DebugOn)
            //{
            //    var quaternion = Quaternion.FromToRotation(new Vector3(0f, 1f, 0f), fromTo);
            //    float angle = quaternion.eulerAngles.z;
            //    arrowForNext.transform.eulerAngles = new Vector3(0f, 0f, angle);
            //    arrowForNext.transform.position = Pos + fromTo.normalized * 0.5f;
            //}

            float multipleX = fromTo.x * moveDir.x;
            float multipleY = fromTo.y * moveDir.y;
            return multipleX < 0f || multipleY < 0f;
        }

    }

    /// <summary>
    /// 움직이는 함수
    /// 목표 지점에 도착하면 true 반환
    /// 아직 도착하지 않았으면 false 반환
    /// </summary>
    /// <returns></returns>
    public bool Path_Move()
    {
        if (null == curTile)
        {
            //if (Vector3.Distance(Pos, moveDestWorldPos) < 0.1f)
            if (IsArrive(Pos, moveDestWorldPos))
            {
                moveDir = Vector3.zero;
                return true;
            }
        }

        //else if (Vector3.Distance(Pos, curTile.worldPosition) < 0.1f)
        else if (IsArrive(Pos, curTile.worldPosition))
        {
            if (null == path || 0 == path.Count)
            {
                curTile = null;
                moveDir = (moveDestWorldPos - Pos).normalized;
            }
            else
            {
                curTile = path.Last();
                path.RemoveAt(path.Count - 1);

                moveDir = (curTile.worldPosition - Pos).normalized;

            }
        }
        IsMoving = true;
        return false;
    }

    /// <summary>
    /// 달리다가 중간에 멈출때 사용하는 함수.
    /// AI상태나 AI패턴은 바꾸지 않고, path, curTile, moveDir 만 초기화한다.
    /// </summary>
    public void Stop_Move()
    {
        path = null;
        curTile = null;
        moveDir = Vector3.zero;
    }

    public bool Find_Path(Vector3 destWorldPos)
    {
        IsMoving = true;
        Vector3Int curPos = TilemapSystem.Instance.WorldToCellPos(commander.Pos);
        Vector3Int curDestPos = TilemapSystem.Instance.WorldToCellPos(destWorldPos);
        moveDestWorldPos = destWorldPos;

        //  출발 지점과 도착지점이 같은 경우
        if (curPos == curDestPos)
        {
            curTile = TilemapSystem.Instance.GetTile(destWorldPos);
            moveDir = (curTile.worldPosition - Pos).normalized;

            return true;
        }

        //if (DebugOn && null != destIndicatorInst)
        //{
        //    destIndicatorInst.transform.position = moveDestWorldPos;
        //}

        path = TilemapSystem.Instance.GetPath(Pos, destWorldPos);

        if (null == path)
        {
            Debug.Log("CommanderAI : Find_Path - path is null");

            return false;
        }

        path.RemoveAt(path.Count() - 1);
        curTile = path.Last();

        if (0 == path.Count())
        {
            curTile = null;
            return false;
        }

        moveDir = (curTile.worldPosition - Pos).normalized;

        return true;
    }

    /// <summary>
    /// false 는 길 못찾음
    /// true 는 길 찾음.
    /// </summary>
    /// <param name="destCellPos"></param>
    /// <returns></returns>
    public bool Find_Path(Vector3Int destCellPos)
    {
        Vector3 destWorldPos = TilemapSystem.Instance.CellToWorldPos(destCellPos);
        Vector3 cellCenterPos = TilemapSystem.Instance.GetTile(destWorldPos).worldPosition;
        return Find_Path(cellCenterPos);
    }

    public bool Find_Path(TileNode node)
    {
        if (null == node)
            return false;

        return Find_Path(node.worldPosition);
    }
}
