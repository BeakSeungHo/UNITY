using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitFSM : UnitFSM
{
    public Machine<NormalUnitFSM> State;

    public enum STATE { IDLE, RUN, ATTACK_IDLE, CAST, DEATH, END };

    public FSM<NormalUnitFSM>[] ArrState = new FSM<NormalUnitFSM>[(int)STATE.END];

    public STATE curState = STATE.END;
    public STATE preState = STATE.END;

    public NormalUnitFSM()
    {
        State = new Machine<NormalUnitFSM>();

        ArrState[(int)STATE.IDLE] = new NormalUnitIdle(this);
        ArrState[(int)STATE.RUN] = new NormalUnitRun(this);
        ArrState[(int)STATE.ATTACK_IDLE] = new NormalUnitAttackIdle(this);
        ArrState[(int)STATE.CAST] = new NormalUnitCast(this);
        ArrState[(int)STATE.DEATH] = new NormalUnitDeath(this);

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

    public override void Command_Move(Vector3 position)
    {
        base.Command_Move(position);

        var tilePos = TilemapSystem.Instance.WorldToCellPos(position);
        if (CommonFSM.UnitTileDest == tilePos)
        {
            return;
        }

        CommonFSM.UnitTileDest = tilePos;

        //UpdatePath(position);
        //IsMove = true;
        CommonFSM.VFAgent.Move(TilemapSystem.Instance.WorldToTilePos(position));
    }

    public override void Command_Move(Vector3Int cellPos)
    {
        base.Command_Move(cellPos);

        UpdatePath(TilemapSystem.Instance.CellToWorldPos(cellPos));
    }

    //public override void Command_Attack(GameObject target)
    //{
    //    AttackTarget = target;
    //}

    //public override void Command_Attack(Character target)
    //{
    //    AttackTarget = target;
    //}

    public override bool IsArrive()
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(MoveDest));
        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    public override bool Scout_Enemy()
    {
        if (CommonType.Skunk == Base.Type)
        {
            if (null != AttackTarget)
                return false;

            //GameObject gameObject = null;

            //if (SquadController.Instance.Find_UnitInRange_ExceptCamp(Pos, Base.Range, Base.MyCamp, Base.AttackType,  out gameObject))
            //{
            //    AttackTarget = gameObject;
            //    return true;
            //}
            Character character = null;

            if (InGameManager.Instance.Find_TargetInRange_ExceptCamp(Pos, Base.Range, Base.MyCamp, Base.AttackType, out character) >= 0)
            {
                AttackTarget = character;
                return true;
            }
            else
                return false;
        }
        else
            return base.Scout_Enemy();
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
        base.Update();

        Run();

        IsCommandMove = false;
        //Debug.Log("NormalUnitFSM Update");
    }

    public override void OnDisable()
    {
        base.OnDisable();

        curState = STATE.END;
        preState = STATE.END;
    }
}