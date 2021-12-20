using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 최초 작성자 : 김영현 (20-09-23)
/// 
/// 공중 유닛의 상태를 관리하는 FSM 클래스
/// 
/// 공중 유닛이 몇개 되지 않기 때문에 switch 문으로 정리하였다.
/// 
/// 
/// </summary>

public class FlyingUnitFSM : UnitFSM
{
    public Machine<FlyingUnitFSM> State;

    public enum STATE { IDLE, RUN, ATTACK_IDLE, CAST, DEATH, SIT, HEAVE, END};

    public FSM<FlyingUnitFSM>[] ArrState = new FSM<FlyingUnitFSM>[(int)STATE.END];

    public STATE curState;
    public STATE preState;

    public float owlSpawnTimeCount = 0f;
    public int firstSpawn = 0;

    /// <summary>
    /// 생성자
    /// State 배열을 초기화 한다.
    /// </summary>

    public FlyingUnitFSM()
    {
        State = new Machine<FlyingUnitFSM>();

        ArrState[(int)STATE.IDLE]           = new FlyingUnitIdle(this);
        ArrState[(int)STATE.RUN]            = new FlyingUnitRun(this);
        ArrState[(int)STATE.ATTACK_IDLE]    = new FlyingUnitAttackIdle(this);
        ArrState[(int)STATE.CAST]           = new FlyingUnitCast(this);
        ArrState[(int)STATE.DEATH]          = new FlyingUnitDeath(this);
        ArrState[(int)STATE.SIT]            = new FlyingUnitSit(this);
        ArrState[(int)STATE.HEAVE]          = new FlyingUnitHeave(this);

        State.SetState(ArrState[(int)STATE.IDLE], this);
    }


    public override bool CanBeTarget()
    {
        if (curState == STATE.DEATH)
            return false;
        else
            return true;
    }

    public override bool IsIdle()
    {
        return STATE.IDLE == curState;
    }

    /// <summary>
    /// 집결 명령 받았을 때 수행할 함수
    /// </summary>
    /// <param name="position">집결 위치</param>
    public override void Command_Move(Vector3 position)
    {
        base.Command_Move(position);
        //FlyingUnitMove(position);

        var tilePos = TilemapSystem.Instance.WorldToCellPos(position);
        if (CommonFSM.UnitTileDest == tilePos)
        {
            return;
        }

        CommonFSM.UnitTileDest = tilePos;

        var node = TilemapSystem.Instance.GetTile(position);
        if (Vector3.Distance(node.worldPosition, Unit.transform.position) > 0.1f)
        {
            IsMove = true;
            MoveDest = TilemapSystem.Instance.WorldToCellPos(position);

            MoveDir = (node.worldPosition - Unit.transform.position).normalized;
        }
    }

    /// <summary>
    /// 공격 명령 받았을 때 수행할 함수
    /// </summary>
    /// <param name="target">공격 대상</param>
    //public override void Command_Attack(GameObject target)
    //{
    //    switch (Base.Type)
    //    {
    //        case CommonType.Pigeon:
    //            Vector3 position = target.transform.position;
    //            Command_Move(position);
    //            break;
    //        case CommonType.Falcon:
    //            AttackTarget = target;
    //            break;
    //        case CommonType.Owl:
    //            break;
    //    }
    //}

    public override void Command_Attack(Character target)
    {
        switch (Base.Type)
        {
            case CommonType.Pigeon:
            case CommonType.Owl:
                Vector3 position = target.transform.position;

                var tilePos = SquadController.Instance.Find_NearestTilePos(Unit, Unit.Base.MyCamp, position);

                position = TilemapSystem.Instance.CellToWorldPos(tilePos);
                var node = TilemapSystem.Instance.GetTile(position);
                if (null == node)
                {
                    Debug.Log("Command_Attack : node is null");
                    return;
                }
                position = node.worldPosition;

                Command_Move(position);
                break;
            case CommonType.Falcon:
                CommandedTarget = target;
                break;
        }
    }

    /// <summary>
    /// 공중 유닛용 이동 함수
    /// </summary>
    /// <param name="destPos">이동 위치</param>
    public void FlyingUnitMove(Vector3 destPos)
    {
        Vector3Int curPos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        Vector3Int curDestPos = TilemapSystem.Instance.WorldToCellPos(destPos);

        if (MoveDest == curDestPos && curPos == MoveStart)
        {
            //Debug.Log("Same Path");
            return;
        }

        MoveStart = TilemapSystem.Instance.WorldToCellPos(transform.position);
        MoveDest = TilemapSystem.Instance.WorldToCellPos(destPos);

        IsMove = true;
        MoveDir = (TilemapSystem.Instance.CellToWorldPos(MoveDest) - Unit.transform.position).normalized;
    }

    /// <summary>
    /// 치료받을 아군 유닛을 찾기 위한 조건자
    /// </summary>
    /// <param name="obj1">비교할 유닛1</param>
    /// <param name="obj2">비교할 유닛2</param>
    /// <returns></returns>
    static public bool ConditionToFindFriendUnit(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2)
            return false;

        CommonUnit unit1 = obj1.GetComponent<CommonUnit>();
        if (null == unit1)
            return false;

        if (!obj2.activeSelf)
            return false;

        Character character = obj2.GetComponent<Character>();
        if (null != character)
        {
            if (character.Base.Data.MaxHp <= character.HP)
                return false;
        }

        return TilemapSystem.Instance.RangeInObject(obj1.transform.position, obj2.transform.position, unit1.Base.Range) != TilemapSystem.Invalid_Range;
        //return Global.Calculate_TileDistance(obj1.transform.position, obj2.transform.position) < unit1.Base.Range;
    }

    /// <summary>
    /// 치료받을 가장 가까운 아군 유닛을 찾기 위한 조건자
    /// </summary>
    /// <param name="obj1">유닛1</param>
    /// <param name="obj2">유닛2</param>
    /// <returns>두 유닛 사이 타일 거리</returns>
    static public float Condition_ToFindFriendUnit(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2)
            return -1f;

        CommonUnit unit1 = obj1.GetComponent<CommonUnit>();
        if (null == unit1)
            return -1f;

        if (!obj2.activeSelf)
            return -1f;

        Character character = obj2.GetComponent<Character>();
        if (null != character)
        {
            if (character.Base.Data.MaxHp <= character.HP)
                return -1f;
        }

        return Global.Calculate_TileDistance(obj1.transform.position, obj2.transform.position);
    }

    /// <summary>
    /// 아군 유닛 찾기
    /// </summary>
    /// <returns></returns>
    public bool Scout_FriendUnit()
    {
        if (null != AttackTarget)
            return false;

        //GameObject foundObject = null;

        ////bool found = SquadController.Instance.Find_CampUnitInRange(Unit.gameObject, Base.MyCamp, out foundObject, ConditionToFindFriendUnit);
        //bool found = SquadController.Instance.Find_NearestCampUnitInRange(Unit.gameObject, Base.MyCamp, out foundObject, Condition_ToFindFriendUnit) < 0;

        //AttackTarget = foundObject;

        Character foundUnit = null;

        //bool found = SquadController.Instance.Find_CampUnitInRange(Unit.gameObject, Base.MyCamp, out foundObject, ConditionToFindFriendUnit);
        bool found = SquadController.Instance.Find_NearestCampUnitInRange(Unit.gameObject, Base.MyCamp, out foundUnit, Condition_ToFindFriendUnit) < 0;

        AttackTarget = foundUnit;

        return found;
    }

    /// <summary>
    /// 비둘기용 도착 확인 함수
    /// </summary>
    /// <returns></returns>
    public bool PigeonArrive()
    {
        Vector3 moveDestWorldPos = TilemapSystem.Instance.CellToWorldPos(MoveDest);

        var node = TilemapSystem.Instance.GetTile(moveDestWorldPos);

        return Global.Calculate_TileDistance(node.worldPosition, moveDestWorldPos) < Base.Range;
        //return TilemapSystem.Instance.RangeInObject(Unit.Pos, moveDestWorldPos, Data.Range - 1) != TilemapSystem.Invalid_Range;
    }

    /// <summary>
    /// 도착 확인 함수
    /// </summary>
    /// <returns></returns>
    public override bool IsArrive()
    {
        Vector3 moveDestWorldPos = TilemapSystem.Instance.CellToWorldPos(MoveDest);

        var node = TilemapSystem.Instance.GetTile(moveDestWorldPos);

        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    /// <summary>
    /// AttackTarget이 사거리 내에 있는지 확인하고, 없으면 타겟이 있는 방향으로 이동 방향을 잡는다.
    /// 지금 당장 공격할 수 있으면 true, 아니면 false 반환.
    /// </summary>
    /// <returns>지금 당장 공격할 수 있으면 true, 아닐경우 false</returns>
    public bool Check_InRangeAndSetMoveDir()
    {
        if (null == AttackTarget)
        {
            return false;
        }

        var fromTilePos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        var toTilePos = TilemapSystem.Instance.WorldToCellPos(AttackTarget.transform.position);

        //  공격하기 위해 자리잡을 위치를 찾음.
        var tilePos = SquadController.Instance.Find_PositionForAttack(Base.MyCamp, fromTilePos, toTilePos, Base.Range, Unit);

        //  자리를 잡을 수 없음.
        if (Global.InvalidTilePos == tilePos)
        {
            Debug.Log("자리를 잡을 수 없음.");
            var nearTilePos = SquadController.Instance.Find_NearestTilePos(Base.MyCamp, toTilePos, Base.PlaceType);
            return false;
        }
        //  찾은 위치가 지금 있는 위치와 같음.
        else if (tilePos == fromTilePos)
        {
            CommonFSM.UnitTileDest = tilePos;

            //  이동할 필요가 없으므로 이동하기 위한 경로 초기화
            curTile = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

            //  타일 위치는 같으나 타일 중앙에 위치하지 않을 수 있으므로, 중앙으로 이동하기 위해 이동 방향과 목적지를 설정헌다.
            MoveDir = curTile.worldPosition - transform.position;
            IsMove = !(MoveDir.magnitude < 0.1f);

            MoveDir.Normalize();
            MoveDest = tilePos;

            return !IsMove;
        }

        CommonFSM.UnitTileDest = tilePos;

        MoveDest = tilePos;

        var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);

        IsMove = true;

        // 현재 경로 노드로 설정된 노드로 향하는 방향벡터 구하기

        var destNode = TilemapSystem.Instance.GetTile(worldPos);

        MoveDir = destNode.worldPosition - transform.position;
        MoveDir.Normalize();



        return false;

    }

    /// <summary>
    /// 상태 변환
    /// </summary>
    /// <param name="newState">바꿀 상태</param>
    public void ChangeFSM(STATE newState)
    {
        if ((int)newState < (int)STATE.END)
            State.Change(ArrState[(int)newState]);
    }

    public void ResetState(STATE state)
    {
        if ((int)state < (int)STATE.END)
            State.ResetState(ArrState[(int)state]);
    }

    /// <summary>
    /// 대모가 쥐 소환하는 함수
    /// </summary>
    public void Owl_SpawnMouse()
    {
        if (firstSpawn < 3)
        {
            ++firstSpawn;
            Spawn_Mouse();
            return;
        }

        owlSpawnTimeCount += Time.deltaTime;

        if (owlSpawnTimeCount >= 5f * (Unit.BuffDebuff.Stim ? 0.5f : 1f))
        {
            owlSpawnTimeCount = 0f;

            Spawn_Mouse();
        }
    }

    /// <summary>
    /// 쥐 소환 함수
    /// </summary>
    public void Spawn_Mouse()
    {
        Camp camp = Unit.Base.MyCamp;

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Unit_Normal);

        CommonUnit commonUnit = pullObject.GetComponent<CommonUnit>();
        if (null == commonUnit)
        {
            Debug.Log("Owl : Mouse Spawn Failed!, commonUnit is null");
            return;
        }

        commonUnit.Ready(camp, CommonType.Mouse, SpawningPoint(), Unit.gameObject);

        SquadController.Instance.Add_Unit(camp, pullObject);

        if (Unit.Base.MyCamp == GameManager.Instance.CommanderList[0])
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 7, 1);

        ChangeFSM(STATE.CAST);
    }

    public Vector3 SpawningPoint()
    {
        var curTilePos = TilemapSystem.Instance.WorldToCellPos(Pos);
        var circularList = StorageBoxes.Instance.CircleSearchList;
        var circleMax = StorageBoxes.Instance.CircleSearchMaxRange;

        for (int i = 0; i < circleMax; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var tilePos = curTilePos + addTile;

                if (TilemapSystem.Instance.IsWalkableTile(tilePos))
                {
                    var destWorldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
                    var node = TilemapSystem.Instance.GetTile(destWorldPos);

                    if (null == node)
                        continue;

                    return node.worldPosition;
                }
            }
        }

        return Global.InvalidWorldPos;
    }

    /// <summary>
    /// 기본 루틴
    /// </summary>

    public void Begin()
    {
        State.Begin();
    }

    public void Run()
    {
        //Debug.Log(curState.ToString());

        if (Unit.HP <= 0)
        {
            ChangeFSM(STATE.DEATH);
        }

        if (null != AttackTarget && !AttackTarget.activeSelf)
            AttackTarget = null;

        State.Run();

    }

    public void Exit()
    {
        State.Exit();
    }

    /// <summary>
    /// PoolManager에서 Pull 할 때 마다 호출할 Ready 함수
    /// </summary>
    public override void Ready()
    {
        base.Ready();
        
        ResetState(STATE.IDLE);

        owlSpawnTimeCount = 0f;
        firstSpawn = 0;

        IsMove = false;
    }

    /// <summary>
    /// 최초 생성시 Ready 함수
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sprite"></param>
    /// <param name="commonFSM"></param>
    
    public override void Ready(GameObject parent, GameObject sprite, CommonUnitFSM commonFSM)
    {
        base.Ready(parent, sprite, commonFSM);

        IsMove = false;
    }

    public override void Start()
    {
        Begin();
    }

    public override void Update()
    {
        Run();
        IsCommandMove = false;
        //Debug.Log("FlyingUnitFSM Update");
    }

    public override void OnDisable()
    {
        base.OnDisable();

        curState = STATE.END;
        preState = STATE.END;
    }
}
