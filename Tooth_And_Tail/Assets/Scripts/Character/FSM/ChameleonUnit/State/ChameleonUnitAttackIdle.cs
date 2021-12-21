using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitAttackIdle : FSM<ChameleonUnitFSM>
{
    private ChameleonUnitFSM ownerFSM;

    public ChameleonUnitAttackIdle(ChameleonUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = ChameleonUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", true);
        //Debug.Log("Chameleon AttackIdle Begin");
    }

    public override void Run()
    {
        if (ownerFSM.IsMove)
        {   
            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.RUN);
            return;
        }

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1f / ownerFSM.Base.AttackSpeed)
        {
            ownerFSM.TimeCount = 0f;
            //if (null != ownerFSM.AttackTarget)
            //{
            //    if (!ownerFSM.AttackTarget.activeSelf)
            //    {
            //        ownerFSM.AttackTarget = null;
            //        ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.IDLE);
            //        return;
            //    }
            //    else
            //    {
            //        if (ownerFSM.CheckTargetInRange())
            //            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
            //    }

            //}
            //else
                ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.IDLE);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = ChameleonUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", false);
    }
}
