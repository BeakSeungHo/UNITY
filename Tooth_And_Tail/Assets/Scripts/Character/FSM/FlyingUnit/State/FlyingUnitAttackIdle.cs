using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitAttackIdle : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    public FlyingUnitAttackIdle(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.ATTACK_IDLE;
        if (CommonType.Falcon != ownerFSM.Base.Type)
            ownerFSM.Animator.SetBool("AttackIdle", true);
    }

    public override void Run()
    {
        switch (ownerFSM.Data.CommonType)
        {
            case CommonType.Pigeon:
                Action_Pigeon();
                break;
            case CommonType.Falcon:
                Action_Falcon();
                break;
            case CommonType.Owl:
                Action_Owl();
                break;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.ATTACK_IDLE;
        if (CommonType.Falcon != ownerFSM.Base.Type)
            ownerFSM.Animator.SetBool("AttackIdle", false);
    }


    private void Action_Pigeon()
    {
        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1f / ownerFSM.Base.AttackSpeed)
        {
            if (null == ownerFSM.AttackTarget || 
                !ownerFSM.AttackTarget.activeSelf)
            {
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
                ownerFSM.AttackTarget = null;
                return;
            }

            Character target = ownerFSM.AttackTarget.GetComponent<Character>();
            if (null == target || target.Base.MaxHp <= target.HP)
            {
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
                ownerFSM.AttackTarget = null;
                return;
            }

            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
        }
    }

    private void Action_Falcon()
    {
        if (ownerFSM.IsMove)
        {
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
            return;
        }

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1f / ownerFSM.Base.AttackSpeed)
        {
            //if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.activeSelf)
            //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
            //else
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
        }
    }

    private void Action_Owl()
    {

    }
}
