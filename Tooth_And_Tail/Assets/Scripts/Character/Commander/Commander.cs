using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// 작성자 : 김영현 (20-09-02)
/// 
/// 유닛을 조종하는 지휘관 클래스
/// 
/// 자신과 충돌한 적 유닛을 가지고 있다가, 공격 명령을 내릴 때 그 유닛을 공격하도록 한다.
/// 
/// 
/// 
/// </summary>

public class Commander : Character
{
    public GameObject Sprite = null;
    public GameObject FSM = null;
    public GameObject Sound = null;
    public SpriteRenderer SpriteRenderer = null;

    public HPCanvas         HpCanvas = null;
    public SpriteRenderer   minimapSprite = null;

    private SquadController squadController = null;

    public Character        AttackTarget = null;

    [HideInInspector] public Camp camp = Camp.End;
    [HideInInspector] public CommanderFSM commanderFSM = null;

    [HideInInspector] public CommonType SelectedUnit;

    public float digTimeCount = 0f;
    public float digTime = 2f;

    public float RespawnTimeCount = 0f;
    public float RespawnTime = 3f;
    public LinkedListNode<BuildingBase> ReturnGristmillNode = null;

    private int     controllSquad = 0;

    // 커맨더의 현재 타일 위치
    Vector2Int goalTilePos;

    public Speech SpeechEvent;
    /// <summary>
    /// 타겟 지정 가능 or 불가능
    /// </summary>
    /// <returns></returns>
    public override bool CanBeTarget()
    {
        return commanderFSM.CanBeTarget;
    }

    public void Change_ControllSquad(CommonType type)
    {
        SelectedUnit = type;
        controllSquad = SquadController.Instance.TypeToSquadNumber[camp][type];
    }

    public void Change_ControllSquad(int squadNum)
    {
        controllSquad = squadNum;
        CommonType type = SquadController.Instance.SquadNumberToType[camp][controllSquad];
        SelectedUnit = type;

        if ((type >= CommonType.Wire && type <= CommonType.Cannon) || type == CommonType.Mole || GameManager.Instance.CurGameMode == GameMode.Campaign)
        {
            TileMarking.Instance.SetTileMark_Size(Base.MyCamp, 1);
        }
        else
        {
            TileMarking.Instance.SetTileMark_Size(Base.MyCamp, 4);
        }
    }

    public void Move_Commander(Vector3 moveDir)
    {
        commanderFSM.moveDir = moveDir;
    }

    public void Rally_Commander(bool inputRally)
    {
        commanderFSM.InputRally = inputRally;
    }

    public void RallyAll_Commander(bool inputRallyAll)
    {
        commanderFSM.InputRallyAll = inputRallyAll;
    }

    public void Command_Move_All()
    {
        //Debug.Log("Commander : Move_All");
        squadController.Command_Move_All(camp, transform.position);
    }

    public void Command_Move()
    {
        //Debug.Log("Commander : Move");
        squadController.Command_Move(camp, controllSquad, transform.position);
    }

    public void Command_Move(int squad)
    {
        squadController.Command_Move(camp, squad, transform.position);
    }

    public void Command_Attack_All()
    {
        squadController.Command_Attack_All(camp, AttackTarget);
    }

    public void Command_Atttack()
    {
        squadController.Command_Attack(camp, controllSquad, AttackTarget);
    }

    public void Command_Attack_All(GameObject target)
    {
        squadController.Command_Attack_All(camp, target);
    }

    public void Comand_Attack(GameObject targert)
    {
        squadController.Command_Attack(camp, controllSquad, targert);
    }

    public void ReturnToBase(bool inputReturn)
    {
        commanderFSM.InputReturn = inputReturn;
    }

    public void Holding()
    {

    }

    public void Digging(bool dig)
    {
        if (dig && !commanderFSM.InputDigging)
            EffectManager.Instance.SmokeEffectEnable(commanderFSM.gameObject, transform.position, 1f, false, ParticleObject.PARTICLETYPE.SMOKE_PLAYER);
        commanderFSM.InputDigging = dig;
    }

    public void MoveLeft_RespawnGristmill()
    {
        var gristmills = BuildingManager.Instance.Buildings[Base.MyCamp][CommonType.Gristmill];

        if (null == ReturnGristmillNode)
            ReturnGristmillNode = gristmills.First;
        else if (!gristmills.Contains(ReturnGristmillNode.Value))
            ReturnGristmillNode = gristmills.First;
        else
        {
            ReturnGristmillNode = ReturnGristmillNode.Previous;
            if (null == ReturnGristmillNode)
                ReturnGristmillNode = gristmills.Last;
        }

        Set_RespawnPos(ReturnGristmillNode.Value.transform.position);
    }

    public void MoveRight_RespawnGristmill()
    {
        var gristmills = BuildingManager.Instance.Buildings[Base.MyCamp][CommonType.Gristmill];

        if (null == ReturnGristmillNode)
            ReturnGristmillNode = gristmills.First;
        else if (!gristmills.Contains(ReturnGristmillNode.Value))
            ReturnGristmillNode = gristmills.First;
        else
        {
            ReturnGristmillNode = ReturnGristmillNode.Next;
            if (null == ReturnGristmillNode)
                ReturnGristmillNode = gristmills.First;
        }

        Set_RespawnPos(ReturnGristmillNode.Value.transform.position);
    }

    public void Set_RespawnPos(Vector3 respawnWorldPosition)
    {
        Vector3 respawnPos = Vector3.zero;

        var tilePos = TilemapSystem.Instance.WorldToCellPos(respawnWorldPosition);

        var searchTileList = StorageBoxes.Instance.CircleSearchList;

        for (int i = 0; i < StorageBoxes.Instance.CircleSearchMaxRange; ++i)
        {
            foreach (var addTile in searchTileList[i])
            {
                var checkTilePos = tilePos + addTile;

                var worldPosition = TilemapSystem.Instance.CellToWorldPos(checkTilePos);
                var node = TilemapSystem.Instance.GetTile(worldPosition);

                if (null == node)
                    continue;

                if (0 == node.Height)
                {
                    transform.position = node.worldPosition;
                    return;
                }
            }
        }
    }

    public void Respawn()
    {
        HP = Base.MaxHp;
        BuffDebuff.StackClear();
    }
    public void SellBuilding(CommonBase commonBase)
    {
        if (commonBase != null)
        {
            switch (commonBase.Type)
            {
                case CommonType.Gristmill:
                    if (commonBase.MyCamp == Base.MyCamp)
                    {
                        if (commonBase.GetComponent<BuildingStateOperator>().CurBuildingState == BuildingState.Construct)
                        {
                            commonBase.GetComponent<BuildingBase>().Sell();
                        }
                    }
                    break;

                case CommonType.CampFire:
                    if (commonBase.MyCamp == Base.MyCamp)
                    {
                        commonBase.GetComponent<BuildingBase>().HP = 0;
                        GameManager.Instance.ChangeFoodCamp(Base.MyCamp, commonBase.Cost);
                        if (Base.MyCamp == GameManager.Instance.CommanderList[0])
                        {
                            //숫자
                            EffectManager.Instance.FontEffectEnable(InGameManager.Instance.Commanders[Base.MyCamp].gameObject, commonBase.Cost, FontEffect.FONTTYPE.MINUSFOOD);
                        }
                    }
                    break;

                case CommonType.Farm:
                    Farm farm = TilemapSystem.Instance.GetTile(transform.position).occupier.GetComponent<Farm>();
                    CommonBase gristmillBase = farm.GetGristmillBase();

                    // 농장을 포함하는 제분소가 건설중이지 않을 경우
                    if (gristmillBase.MyCamp == Base.MyCamp && gristmillBase.GetComponent<BuildingStateOperator>().CurBuildingState != BuildingState.Construct)
                    {
                        if (farm.GetState() == Farm.FarmState.Cultivation)
                            commonBase.GetComponent<BuildingBase>().Sell();
                    }
                    break;
                case CommonType.Turret:
                case CommonType.Balloon:
                case CommonType.Cannon:
                case CommonType.Wire:
                case CommonType.Mine:
                case CommonType.MoleeMerge:
                case CommonType.WarrenT1:
                case CommonType.WarrenT2:
                case CommonType.WarrenT3:
                    if(commonBase.MyCamp == Base.MyCamp)
                        commonBase.GetComponent<BuildingBase>().Sell();
                    break;
            }
        }
    }

    // 건물 판매 및 중립 건물 점령
    public void OccupyNeutral(BuildingBase buildingBase)
    {

        if (buildingBase != null)
        {
            // 농장의 경우 식량이 없어도 큐에 넣을 수 있기 때문에 식량 검사를 농장 점령 함수에서 처리하게 해놨고
            // 제분소가 점령이 안된 경우 농장을 통하여 제분소를 점령할 수 있기 때문에 중립인 농장의 경우는 식량 검사를 해야 한다.
            if (buildingBase.Base.Type != CommonType.Farm || (buildingBase.farm != null && buildingBase.farm.GristmillBuildingBase.Base.MyCamp == Camp.End))
            {
                if (GameManager.Instance.CampFoodDic[camp] < BuildingBase.NeutralBuildCost)
                {
                    Debug.Log("식량이 부족합니다");
                    return;
                }
            }
            switch (buildingBase.Base.Type)
            {
                case CommonType.Gristmill:

                    if (buildingBase.Base.MyCamp == Camp.End)
                    {
                        buildingBase.Base.MyCamp = camp;
                        buildingBase.ChangeState(BuildingState.Construct);
                    }
                    break;

                case CommonType.CampFire:
                    if (buildingBase.Base.MyCamp == Camp.End && buildingBase.GetAnimator().GetBool("Exhasted") == false)
                    {
                        GameManager.Instance.ChangeFoodCamp(Base.MyCamp, -buildingBase.Base.Cost);
                        buildingBase.OccupyNeutral(Base.MyCamp);
                        if (Base.MyCamp == GameManager.Instance.CommanderList[0])
                        {
                            //숫자
                            EffectManager.Instance.FontEffectEnable(gameObject, buildingBase.Base.Cost, FontEffect.FONTTYPE.MINUSFOOD);
                        }
                    }
                    break;

                case CommonType.Farm:
                    Farm farm = TilemapSystem.Instance.GetTile(transform.position).occupier.GetComponent<Farm>();
                    CommonBase gristmillBase = farm.GetGristmillBase();
                    BuildingBase gristmillBuildingBase = farm.GristmillBuildingBase;
                    if (gristmillBase.MyCamp == Camp.End)
                    {
                        gristmillBuildingBase.Base.MyCamp = camp;
                        gristmillBuildingBase.ChangeState(BuildingState.Construct);
                    }
                    // 농장을 포함하는 제분소가 건설중이지 않을 경우
                    if (gristmillBase.MyCamp == Base.MyCamp && gristmillBase.GetComponent<BuildingStateOperator>().CurBuildingState != BuildingState.Construct)
                    {
                        if (farm.GetState() == Farm.FarmState.Idle && farm.exhaust == false)
                        {
                            farm.Cultivation(Base.MyCamp);
                        }
                        else
                            Debug.Log("건설이 이미 완료되었거나 이미 소진된 농장입니다.");
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("빌딩베이스가 null입니다");
        }
    }

    /// <summary>
    /// 짓기 or 팔기 버튼을 눌렀을 때 호출할 함수
    /// </summary>
    public void BuildBuilding()
    {
        // Debug.Log("BuildOrSell 함수 호출됨.");

        // TileMark중 월드좌표의 Y축기준으로 맨 밑에 있는 타일에 건물을 건설하기 위해 해당 위치를 저장할 변수
        Vector3 LowestTile = Vector3.zero;


        List<GameObject> tileMarks = TileMarking.Instance.TileMarks[Base.MyCamp];

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

        // 건설 가능한 타일이면 건설
        if (true == TileMarking.Instance.IsBuildable[Base.MyCamp])
        {
            Build(LowestTile);
            return;
        }
        // 해당 타일을 점유하고 있는 건물이 있을 경우 중립건물인지 판별하여 점령한다
        else
        {
            BuildingBase buildingBase = (BuildingBase)TilemapSystem.Instance.GetTile(transform.position).occupier;
            OccupyNeutral(buildingBase);
        }
    }

    // 유닛 소환 함수
    public void UnitSummons(Camp GenCamp, CommonType UnitType)
    {
        //  죽으면 소환 안함,
        if (CommanderFSM.STATE.DEATH == commanderFSM.curState ||
            CommanderFSM.STATE.RESPAWN == commanderFSM.curState)
            return;

        int UnitCost = SceneStarter.Instance.commonElements.CommonDataList[(int)UnitType].Cost +
                        SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(UnitType,
                         SceneStarter.Instance.userElements.GetLevel(UnitType)).Cost;

        if (GameManager.Instance.CampFoodDic[Base.MyCamp] >= UnitCost)
            GameManager.Instance.CampFoodDic[Base.MyCamp] -= UnitCost;
        else
            return;

        Pool_ObjType poolType = SceneStarter.Instance.commonElements.CommonDataList[(int)UnitType].PoolType;

        GameObject pullObj = PoolManager.Instance.PullObject(poolType);
        if (poolType == Pool_ObjType.Unit_Normal)
        {
            CommonUnit unit = pullObj.GetComponent<CommonUnit>();

            if (null == unit)
                return;

            unit.Ready(GenCamp, UnitType, transform.position);
            SquadController.Instance.Add_Unit(GenCamp, pullObj);
        }
        else
        {
            Vector3 position = TilemapSystem.Instance.GetTile(transform.position).worldPosition;
            BuildingBase buildingBase = pullObj.GetComponent<BuildingBase>();
            buildingBase.transform.position = position;
            buildingBase.Base.MyCamp = GenCamp;
            buildingBase.Product_Type = CommonType.End;
            if (poolType == Pool_ObjType.Warrens || poolType == Pool_ObjType.MoleeMerge)
            {
                if (UnitType >= CommonType.Squirrel && UnitType <= CommonType.Pigeon)
                    buildingBase.Base.Type = CommonType.WarrenT1;
                else if (UnitType == CommonType.Mole)
                    buildingBase.Base.Type = CommonType.MoleeMerge;
                else if (UnitType >= CommonType.Ferret && UnitType <= CommonType.Snake)
                    buildingBase.Base.Type = CommonType.WarrenT2;
                else if (UnitType >= CommonType.Boar && UnitType <= CommonType.Fox)
                    buildingBase.Base.Type = CommonType.WarrenT3;
                buildingBase.Product_Type = UnitType;
            }
            else
                buildingBase.Base.Type = UnitType;
            buildingBase.Make_Building();
        }

        switch (UnitType)
        {
            case CommonType.Owl:
                GameManager.Instance.MissionOwlCount++;
                break;
            case CommonType.Wolf:
                GameManager.Instance.MissionWolfCount++;
                break;
            case CommonType.Chameleon:
                GameManager.Instance.MissionChameleonCount++;
                break;
        }
    }

    /// <summary>
    /// 건물 짓기 함수
    /// </summary>
    public void Build(Vector3 buildPos)
    {
        int requireFood = 0;
        int extraFood = 0;

        if (SelectedUnit >= CommonType.Squirrel && SelectedUnit <= CommonType.Fox)
            extraFood = SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(SelectedUnit, SceneStarter.Instance.userElements.GetLevel(SelectedUnit)).BuildCost;
        if (SelectedUnit >= CommonType.Squirrel && SelectedUnit <= CommonType.Mole)
        {
            requireFood = SceneStarter.Instance.GetData(CommonType.WarrenT1).Cost;
        }
        else if (SelectedUnit >= CommonType.Ferret && SelectedUnit <= CommonType.Snake)
        {
            requireFood = SceneStarter.Instance.GetData(CommonType.WarrenT2).Cost;
        }
        else if (SelectedUnit >= CommonType.Boar && SelectedUnit <= CommonType.Fox)
        {
            requireFood = SceneStarter.Instance.GetData(CommonType.WarrenT3).Cost;
        }
        else
            requireFood = SceneStarter.Instance.GetData(SelectedUnit).Cost;

        requireFood += extraFood;

        if (GameManager.Instance.GetFood(Base.MyCamp) < requireFood)
        {
            Debug.Log(Base.MyCamp + "의 식량이 부족합니다");
            return;
        }

        // 
        GameManager.Instance.ChangeFoodCamp(Base.MyCamp, -requireFood);

        if (Base.MyCamp == GameManager.Instance.CommanderList[0])
        {
            //숫자
            EffectManager.Instance.FontEffectEnable(gameObject, requireFood, FontEffect.FONTTYPE.MINUSFOOD);
        }

        Vector3 buildingPos = Vector3.zero;

        bool bigSize = false;
        bool useCollider = false;

        GameObject buildings;
        switch (SelectedUnit)
        {
            case CommonType.Turret:
            case CommonType.Cannon:
                buildings = PoolManager.Instance.PullObject(Pool_ObjType.Building_Defender);
                useCollider = true;
                break;

            case CommonType.Balloon:
            case CommonType.Wire:
            case CommonType.Mine:
                buildings = PoolManager.Instance.PullObject(Pool_ObjType.Building_Defender);
                break;

            case CommonType.Mole:
                buildings = PoolManager.Instance.PullObject(Pool_ObjType.MoleeMerge);
                break;

            default:
                buildings = PoolManager.Instance.PullObject(Pool_ObjType.Warrens);
                bigSize = true;
                break;
        }

        if (bigSize)
            buildings.transform.position = buildPos;
        else
            buildings.transform.position = TilemapSystem.Instance.GetTile(transform.position).worldPosition;

        Collider2D collider = buildings.GetComponent<Collider2D>();

        if (collider != null)
        {
            if (useCollider)
                collider.enabled = true;
            else
                collider.enabled = false;
        }

        BuildingBase b_base;
        b_base = buildings.GetComponent<BuildingBase>();

        CommonBase c_base = b_base.Base;
        c_base.MyCamp = camp;

        switch (SelectedUnit)
        {
            case CommonType.Turret:
            case CommonType.Cannon:
            case CommonType.Balloon:
            case CommonType.Wire:
            case CommonType.Mine:
                c_base.Type = SelectedUnit;
                break;
            default:
                b_base.Product_PoolType = GetComponent<CommonBase>().GetPoolType(SelectedUnit);
                b_base.Product_Type = SelectedUnit;

                if (SelectedUnit == CommonType.Mole)
                {
                    c_base.Type = CommonType.MoleeMerge;
                }
                else
                {
                    c_base.Type = CommonType.WarrenT3 - (c_base.GetData(SelectedUnit).UnitPerBuliding - 1);

                }
                break;
        }

        b_base.Make_Building();
    }
    /// <summary>
    /// 건물 팔기 함수
    /// </summary>
    public void SellBuilding()
    {
        Debug.Log("SellBuilding 함수 호출됨.");

        var buildingBase = AttackTarget.GetComponent<BuildingBase>();

        switch (buildingBase.Base.Type)
        {
            case CommonType.Gristmill:
                break;
            case CommonType.Farm:
                break;
            case CommonType.Turret:
                break;
        }
    }

    public void Start()
    {
        squadController = SquadController.Instance;
    }

    public override void Awake()
    {
        base.Awake();
        camp = GetComponent<CommonBase>().MyCamp;
        
        commanderFSM = FSM.GetComponent<CommanderFSM>();
    }

    public void Ready(Camp camp)
    {
        Base.MyCamp = camp;
        this.camp = camp;
        Animator anim = Sprite.GetComponent<Animator>();

        anim.runtimeAnimatorController = SceneStarter.Instance.animatorElements.ComAniDic[camp];

        SpriteRenderer test = Sprite.GetComponent<SpriteRenderer>();

        if (CampaignManager.Instance != null)
        {
            transform.position = CampaignManager.Instance.startPosition.position;
        }

        HP = Base.MaxHp;

        HpCanvas.Ready();
        SetMiniSpriteColor(camp);
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log(null == AttackTarget ? "AttackTarget is null" : "AttackTarget is not null");

        Vector2Int tilePos = TilemapSystem.Instance.WorldToTilePos(gameObject.transform.position);

        TileNode curTileNode = TilemapSystem.Instance.GetTile(tilePos);
        
        // 테스트용
        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameManager.Instance.ChangeFoodCamp(Base.MyCamp, 60);
            if (Base.MyCamp == GameManager.Instance.CommanderList[0])
            {
                //숫자
                EffectManager.Instance.FontEffectEnable(gameObject, 60, FontEffect.FONTTYPE.PLUSFOOD);
            }
        }

        if (Input.GetKeyDown(KeyCode.F3) && Base.MyCamp == Camp.Archimedes)
        {
            BuildingBase buildingBase = (BuildingBase)TilemapSystem.Instance.GetTile(transform.position).occupier;
            if (buildingBase != null)
            {
                switch (buildingBase.Base.Type)
                {
                    case CommonType.Gristmill:
                        if (buildingBase.Base.MyCamp == Camp.End)
                        {
                            int curFood = GameManager.Instance.GetFood(camp);
                            if (curFood < buildingBase.Base.Data.Cost)
                            {
                                GameManager.Instance.ChangeFoodCamp(camp, buildingBase.Base.Data.Cost);
                                if (Base.MyCamp == GameManager.Instance.CommanderList[0])
                                {
                                    //숫자
                                    EffectManager.Instance.FontEffectEnable(gameObject, buildingBase.Base.Data.Cost, FontEffect.FONTTYPE.PLUSFOOD);
                                }
                                Debug.Log("식량이 부족합니다");
                                return;
                            }

                            Animator animator = buildingBase.transform.Find("Body").GetComponent<Animator>();
                            buildingBase.Base.MyCamp = Base.MyCamp;
                            animator.SetBool("Idle", false);
                            animator.SetBool("Idle_Des", false);
                            animator.SetBool("Idle_Neutral", false);
                            //commonBase.GetComponent<BuildingBase>().GetAnimator().SetBool("Idle_Des", false);
                            buildingBase.GetComponent<BuildingStateOperator>().ChangeState(BuildingState.Construct);
                        }
                        else
                        {
                            if (buildingBase.GetComponent<BuildingStateOperator>().CurBuildingState == BuildingState.Construct)
                            {
                                buildingBase.GetComponent<BuildingBase>().Sell();
                            }
                        }
                        break;

                    case CommonType.Farm:
                        Farm farm = TilemapSystem.Instance.GetTile(transform.position).occupier.GetComponent<Farm>();
                        CommonBase gristmillBase = farm.GetGristmillBase();
                        if (gristmillBase.MyCamp == Base.MyCamp && gristmillBase.GetComponent<BuildingStateOperator>().CurBuildingState != BuildingState.Construct)
                        {
                            if (farm.GetState() == Farm.FarmState.Idle)
                            {
                                farm.Cultivation(Base.MyCamp);
                            }
                            else if (farm.GetState() == Farm.FarmState.Cultivation)
                                buildingBase.GetComponent<BuildingBase>().Sell();
                            else
                                Debug.Log("건설이 이미 완료되었습니다");
                        }
                        break;
                    case CommonType.Turret:
                    case CommonType.Balloon:
                    case CommonType.Cannon:
                    case CommonType.Wire:
                    case CommonType.Mine:
                    case CommonType.MoleeMerge:
                    case CommonType.WarrenT1:
                    case CommonType.WarrenT2:
                    case CommonType.WarrenT3:
                        buildingBase.GetComponent<BuildingBase>().Sell();
                        break;
                }
            }
            else
            {
                List<GameObject> tileMarks = TileMarking.Instance.TileMarks[Base.MyCamp];

                List<Vector2Int> nodes = new List<Vector2Int>();

                Vector3 LowestTile = Vector3.zero;

                float minY = 999999;

                for (int i = 0; i < tileMarks.Count; i++)
                {

                    Vector2Int tPos = TilemapSystem.Instance.WorldToTilePos(tileMarks[i].transform.position);
                    Vector3 tileWorldPos = TilemapSystem.Instance.GetTile(tilePos).worldPosition;
                    if (tileWorldPos.y < minY)
                    {
                        LowestTile = tileWorldPos;
                        minY = tileWorldPos.y;
                    }
                    nodes.Add(tPos);
                }
                Build(LowestTile);
            }
        }

        // 테스트용 끝

        Update_AttackTarget();

        Recovery();

        FogOfWar.Instance.CheckSprite(transform.position, HPCanvas, minimapSprite.gameObject, SpriteRenderer);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }


    /// <summary>
    /// 커맨더가 있는 타일을 포함하여 주변 9타일에 있는 유닛, 빌딩중에 가장 가까이 있는 오브젝트를 AttackTarget으로 설정한다.
    /// </summary>
    private void Update_AttackTarget()
    {
        AttackTarget = null;
        //
        Vector2Int tilePos = TilemapSystem.Instance.WorldToTilePos(gameObject.transform.position);
        TileNode curTileNode = TilemapSystem.Instance.GetTile(tilePos);
        if (null == curTileNode)
            return;

        //  최소 거리
        float minDist = -1f;

        //  체크할 타일 리스트
        List<TileNode> TileList = new List<TileNode>();
        TileList.Capacity = 9;

        TileList.Add(curTileNode);
        for (int i = 0; i < curTileNode.Neighbors.Count; ++i)
        {
            TileList.Add(curTileNode.Neighbors[i]);
        }

        //  순회
        for (int i = 0; i < TileList.Count; ++i)
        {
            var tileWorldPos = TileList[i].worldPosition;
            var nodeCellPos = TilemapSystem.Instance.WorldToCellPos(tileWorldPos);

            var unitSet = StorageBoxes.Instance.TileObjects[nodeCellPos].OccupiedUnitSet;

            //  유닛 체크
            foreach (var unit in unitSet)
            {
                if (unit.Base.MyCamp != Base.MyCamp)
                {
                    float unitDist = (unit.Pos - transform.position).magnitude;
                    if (minDist < 0 || minDist > unitDist)
                    {
                        minDist = unitDist;
                        AttackTarget = unit;
                    }
                }
            }

            //  빌딩 체크
            if (null != TileList[i].occupier)
            {
                var building = TileList[i].occupier.gameObject;

                var buildingBase = TileList[i].occupier;
            
                if (buildingBase.Base.MyCamp == Base.MyCamp)
                    continue;
                if (Camp.End == buildingBase.Base.MyCamp && 
                    (CommonType.Farm == buildingBase.Base.Type || 
                    CommonType.Gristmill == buildingBase.Base.Type || 
                    CommonType.CampFire == buildingBase.Base.Type))
                    continue;

                float buildingDist = (building.transform.position - transform.position).magnitude;

                if (minDist < 0 || minDist > buildingDist)
                {
                    minDist = buildingDist;
                    AttackTarget = building.GetComponent<BuildingBase>();
                }
            }
        }

        //  가까이 있는게 없음
        if (minDist < 0)
        {
            AttackTarget = null;
        }
    }

    // 미니맵 스프라이트 색 정하는 함수
    public void SetMiniSpriteColor(Camp camp)
    {
        switch (camp)
        {
            case Camp.Bellafide:
                minimapSprite.color = Global.MinimapColorCommanderBellafide;
                break;
            case Camp.Hopper:
                minimapSprite.color = Global.MinimapColorCommanderHopper;
                break;
            case Camp.Quartermaster:
                minimapSprite.color = Global.MinimapColorCommanderQuartermaster;
                break;
            case Camp.Archimedes:
                minimapSprite.color = Global.MinimapColorCommanderArchimedes;
                break;
        }
    }

    public void LevelUp()
    {
        HP += 9.9f;
        if (HP >= Base.MaxHp)
            HP = Base.MaxHp;

        //  폰트 이펙트 추가

    }
}
