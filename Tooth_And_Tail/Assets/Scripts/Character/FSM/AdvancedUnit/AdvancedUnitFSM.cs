using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitFSM : UnitFSM
{
    public Machine<AdvancedUnitFSM> State;

    public enum STATE { IDLE, RUN, ATTACK_IDLE, CAST, CAST_RUN, DEATH, END };

    public FSM<AdvancedUnitFSM>[] ArrState = new FSM<AdvancedUnitFSM>[(int)STATE.END];

    public STATE curState = STATE.END;
    public STATE preState = STATE.END;

    public bool     isFired = false;
    public float    BadgerCoolDown = 0f;
    public float    BadgerFireSpeed = 1f;
    private Sound   boarFlameSound = null;
    private List<AudioClip> boarFlameAudios = null;
    private Sound   VoiceSound = null;

    public float AttackSpeed { get { return Base.AttackSpeed * BadgerFireSpeed; } }

    public bool AttackEffect = false;
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

    public AdvancedUnitFSM()
    {
        State = new Machine<AdvancedUnitFSM>();

        ArrState[(int)STATE.IDLE]           = new AdvancedUnitIdle(this);
        ArrState[(int)STATE.RUN]            = new AdvancedUnitRun(this);
        ArrState[(int)STATE.ATTACK_IDLE]    = new AdvancedUnitAttackIdle(this);
        ArrState[(int)STATE.CAST]           = new AdvancedUnitCast(this);
        ArrState[(int)STATE.CAST_RUN]       = new AdvancedUnitCastRun(this);
        ArrState[(int)STATE.DEATH]          = new AdvancedUnitDeath(this);

        State.SetState(ArrState[(int)STATE.IDLE], this);

        if (null == boarFlameAudios)
            boarFlameAudios = SceneStarter.Instance.soundElements.UnitSoundDic[CommonType.Boar][UnitSoundType.Attack];

    }

    public override void Command_Move(Vector3 position)
    {
        base.Command_Move(position);


        var tilePos = TilemapSystem.Instance.WorldToCellPos(position);
        if (CommonFSM.UnitTileDest == tilePos)
        {
            return;
        }

        CommonFSM.UnitTileDest = tilePos;
        CommonFSM.VFAgent.Move(TilemapSystem.Instance.WorldToTilePos(position));
    }

    //public override void Command_Attack(GameObject target)
    //{
    //    AttackTarget = target;
    //}

    public override void Command_Attack(Character target)
    {
        //AttackTarget = target;
        CommandedTarget = target;
    }

    public override bool IsArrive()
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(MoveDest));

        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    /// <summary>
    /// 버프 받을 아군 유닛을 찾기 위환 조건자
    /// </summary>
    /// <param name="obj1">비교할 유닛1</param>
    /// <param name="obj2">비교할 유닛2</param>
    /// <returns>찾으면 true 못찾으면 false </returns>
    static public bool ConditionToFindFriendUnit(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2)
            return false;

        CommonUnit unit1 = obj1.GetComponent<CommonUnit>();
        if (null == unit1)
            return false;

        if (!obj2.activeSelf)
            return false;

        BuffDebuff buffDebuff = obj2.GetComponent<BuffDebuff>();
        if (null != buffDebuff)
        {
            if (buffDebuff.Stim)
                return false;
        }

        Character character = obj2.GetComponent<Character>();
        if (null != character)
        {
            if (CommonType.Wolf == character.Base.Type)
                return false;
        }

        return TilemapSystem.Instance.RangeInObject(obj1.transform.position, obj2.transform.position, unit1.Base.Range) != TilemapSystem.Invalid_Range;
    }

    static public float DistanceForFindFriendUnit(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2)
            return -1f;

        CommonUnit unit1 = obj1.GetComponent<CommonUnit>();
        if (null == unit1)
            return -1f;

        if (!obj2.activeSelf)
            return -1f;

        BuffDebuff buffDebuff = obj2.GetComponent<BuffDebuff>();
        if (null != buffDebuff)
        {
            if (buffDebuff.Stim)
                return -1f;
        }

        Character character = obj2.GetComponent<Character>();
        if (null != character)
        {
            if (CommonType.Wolf == character.Base.Type)
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
        Character foundUnit = null;

        //bool found = SquadController.Instance.Find_CampUnitInRange(Unit.gameObject, Base.MyCamp, out foundObject, ConditionToFindFriendUnit);
        //bool found = SquadController.Instance.Find_NearestCampUnitInRange(Unit.gameObject, Base.MyCamp, out foundUnit, DistanceForFindFriendUnit) < 0;
        bool found = InGameManager.Instance.Find_TargetInRange_ForWolfBuff(Pos, Base.Range, Base.MyCamp, Base.AttackType, out foundUnit) > 0f;

        AttackTarget = foundUnit;
        return found;
    }

    /// <summary>
    /// 오소리 공격 안하고 있을 때 공속 초기화용 함수
    /// </summary>
    public void Badger_Cooling()
    {
        BadgerCoolDown += Time.deltaTime;
        if (BadgerCoolDown >= 2f)
        {
            BadgerFireSpeed = 1f;
        }
    }

    /// <summary>
    /// 오소리 공속 업 함수
    /// </summary>
    public void Badger_FireSpeedUp()
    {
        BadgerCoolDown = 0f;
        BadgerFireSpeed += Time.deltaTime * 5f / 6f;

        if (BadgerFireSpeed > 5f)
            BadgerFireSpeed = 5f;

        //Debug.Log("Fire Speed : " + BadgerFireSpeed.ToString());
    }

    /// <summary>
    /// 멧돼지 타일 히트 쿨 타임
    /// </summary>
    public void Boar_FireCoolDown()
    {
        if (isFired)
        {
            TimeCount += Time.deltaTime;
            if (TimeCount >= 1f)
            {
                isFired = false;
                TimeCount = 0f;
            }
        }
    }
    /// <summary>
    /// Wolf 용 버프 쿨타임
    /// </summary>
    public void Wolf_CoolDown()
    {

    }

    public void Boar_FlameThrow(Vector3Int cellPos)
    {
        var worldPos = TilemapSystem.Instance.CellToWorldPos(cellPos);

        var node = TilemapSystem.Instance.GetTile(worldPos);

        if (null == node)
            return;

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);

        if (null == pullObject)
        {
            Debug.Log("Boar Cast : pullObject is null");
            return;
        }

        TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();

        if (null == tileHitObject)
        {
            Debug.Log("Boar Cast : tileHitObject is null");
            return;
        }

        tileHitObject.Ready_Boar(Base.MyCamp, Data.Damage, 1f, node.worldPosition, Unit);
    }

    /// <summary>
    /// 멧돼지 화염 방사 함수
    /// </summary>
    /// <param name="gridPos">불 지를 타일</param>
    public void Boar_FlameThrow(Vector2Int gridPos)
    {
        var node = TilemapSystem.Instance.GetTile(gridPos);
        if (null == node)
            return;

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);

        if (null == pullObject)
        {
            Debug.Log("Boar Cast : pullObject is null");
            return;
        }

        TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();

        if (null == tileHitObject)
        {
            Debug.Log("Boar Cast : tileHitObject is null");
            return;
        }

        tileHitObject.Ready_Boar(Base.MyCamp, Data.Damage, 1f, node.worldPosition, Unit);
    }

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

    public void Begin()
    {
        State.Begin();
    }

    public void Run()
    {
        if (Unit.HP <= 0)
        {
            ChangeFSM(STATE.DEATH);
        }
        if (CommonType.Boar == Base.Type)
            Boar_FireCoolDown();
        else if (CommonType.Wolf == Base.Type)
            Wolf_CoolDown();
        State.Run();
    }

    public void Exit()
    {
        State.Exit();
    }

    public override void Ready()
    {
        base.Ready();

        IsMove = false;

        ResetState(STATE.IDLE);
    }

    public override void Ready(GameObject parent, GameObject sprite , CommonUnitFSM commonFSM)
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
        //Debug.Log("AdvancedUnitFSM Update");
    }
    public override void OnDisable()
    {
        base.OnDisable();

        curState = STATE.END;
        preState = STATE.END;

        isFired = false;
        BadgerCoolDown = 0f;
        BadgerFireSpeed = 1f;

        AttackEffect = false;
    }

    public void Start_BoarFlameSound()
    { 
        if (null != boarFlameSound)
        {
            if (boarFlameSound.gameObject.activeSelf)
                boarFlameSound.Stop();
            boarFlameSound = null;
        }
        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, boarFlameAudios[2], boarFlameSound);
    }

    public void Loop_BoarFlameSound()
    {
        if (null != boarFlameSound)
        {
            if (!boarFlameSound.gameObject.activeSelf)
            {
                boarFlameSound = null;
            }
        }
        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, boarFlameAudios[0], boarFlameSound);
    }

    public void End_BoarFlameSound()
    {
        if (null != boarFlameSound)
        {
            if (boarFlameSound.gameObject.activeSelf)
                boarFlameSound.Stop();
            boarFlameSound = null;
        }
        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, boarFlameAudios[1], boarFlameSound);
    }
}
