using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitFSM : UnitFSM
{
    public Machine<ChameleonUnitFSM> State;

    public enum STATE { IDLE, RUN, ATTACK_IDLE, CAST, DEATH, SPAWN, END };

    public FSM<ChameleonUnitFSM>[] ArrState = new FSM<ChameleonUnitFSM>[(int)STATE.END];

    public STATE curState = STATE.END;
    public STATE preState = STATE.END;

    public bool         hideState = false;
    public float        hideTime = 0f;
    public SpriteRenderer spriteRenderer = null;

    public override bool CanBeTarget()
    {
        if (curState == STATE.DEATH)
            return false;
        else
            return !hideState;
    }

    public override bool IsIdle()
    {
        return STATE.IDLE == curState;
    }

    public ChameleonUnitFSM()
    {
        State = new Machine<ChameleonUnitFSM>();

        ArrState[(int)STATE.IDLE] = new ChameleonUnitIdle(this);
        ArrState[(int)STATE.RUN] = new ChameleonUnitRun(this);
        ArrState[(int)STATE.ATTACK_IDLE] = new ChameleonUnitAttackIdle(this);
        ArrState[(int)STATE.CAST] = new ChameleonUnitCast(this);
        ArrState[(int)STATE.DEATH] = new ChameleonUnitDeath(this);

        State.SetState(ArrState[(int)STATE.IDLE], this);
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

    //public override void Command_Attack(Character target)
    //{
    //    AttackTarget = target;
    //}

    public bool Check_EnemyInRange()
    {
        return false;
    }

    public override bool IsArrive()
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(MoveDest));
        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    public void Hiding()
    {
        //Debug.Log("Hiding...");
        if (!hideState)
        {
            hideTime += Time.deltaTime;
            //Debug.Log("Hiding : Counting - hideTime : " + hideTime.ToString());
            if (hideTime >= 1f)
            {
                hideTime = 0f;
                hideState = true;
                spriteRenderer.color = GameManager.Instance.CommanderList[0] == Base.MyCamp ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 0f);
                //Debug.Log("Hiding!");
            }
        }
        else
        {
            if (GameManager.Instance.CommanderList[0] == Base.MyCamp)
            {
                if (spriteRenderer.color.a != 0.5f)
                    spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                if (spriteRenderer.color.a != 0f)
                    spriteRenderer.color = new Color(1, 1, 1, 0f);
            }
        }
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

        hideTime = 0f;
        hideState = false;

        ResetState(STATE.IDLE);
    }

    public override void Ready(GameObject parent, GameObject sprite, CommonUnitFSM commonFSM)
    {
        base.Ready(parent, sprite, commonFSM);
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        IsMove = false;
        hideTime = 0f;
        hideState = false;
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

        hideState = false;
        hideTime = 0f;
    }
}