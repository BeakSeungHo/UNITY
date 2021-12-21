using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitFSM : UnitFSM
{
    public Machine<MouseUnitFSM> State;

    public enum STATE { SPAWN, IDLE, RUN, ATTACK_IDLE, CAST, DEATH, END };

    public FSM<MouseUnitFSM>[] ArrState = new FSM<MouseUnitFSM>[(int)STATE.END];

    public STATE curState;
    public STATE preState;

    public float lifeCount = 0f;
    public GameObject Owl = null;

    public MouseUnitFSM()
    {
        State = new Machine<MouseUnitFSM>();

        ArrState[(int)STATE.SPAWN]          = new MouseUnitSpawn(this);
        ArrState[(int)STATE.IDLE]           = new MouseUnitIdle(this);
        ArrState[(int)STATE.RUN]            = new MouseUnitRun(this);
        ArrState[(int)STATE.ATTACK_IDLE]    = new MouseUnitAttackIdle(this);
        ArrState[(int)STATE.CAST]           = new MouseUnitCast(this);
        ArrState[(int)STATE.DEATH]          = new MouseUnitDeath(this);

        State.SetState(ArrState[(int)STATE.SPAWN], this);
    }

    public override bool IsIdle()
    {
        return STATE.IDLE == curState;
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

        //IsMove = true;

        //MoveDest = position;
        //MoveDir = (MoveDest - Unit.transform.position).normalized;

        //UpdatePath(position);

        //Debug.Log(Data.CommonType.ToString() + " : Command_Move");
        CommonFSM.VFAgent.Move(TilemapSystem.Instance.WorldToTilePos(position));
    }

    //public override void Command_Attack(GameObject target)
    //{
    //    AttackTarget = target;
    //}

    public override void Command_Attack(Character target)
    {
        AttackTarget = target;
    }

    public bool Check_EnemyInRange()
    {
        return false;
    }

    public override bool IsArrive()
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(MoveDest));
        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    public bool LifeCounting()
    {
        if (null != Owl && !Owl.activeSelf)
        {
            Owl = null;

            lifeCount = 0f;

            ChangeFSM(STATE.DEATH);

            return true;
        }

        lifeCount += Time.deltaTime;

        if (lifeCount >= 30f)
        {
            lifeCount = 0f;

            ChangeFSM(STATE.DEATH);

            return true;
        }
        return false;
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

        State.Run();

        if (null != AttackTarget && !AttackTarget.activeSelf)
            AttackTarget = null;
    }

    public void Exit()
    {
        State.Exit();
    }

    public override void Ready()
    {
        base.Ready();

        IsMove = false;

        lifeCount = 0f;

        ResetState(STATE.SPAWN);
    }

    public override void Set_OwlOfMouse(GameObject owl)
    {
        base.Set_OwlOfMouse(owl);

        Owl = owl;
    }

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
        //Debug.Log("NormalUnitFSM Update");
    }
    public override void OnDisable()
    {
        base.OnDisable();

        curState = STATE.END;
        preState = STATE.END;

        lifeCount = 0f;
    }
}
