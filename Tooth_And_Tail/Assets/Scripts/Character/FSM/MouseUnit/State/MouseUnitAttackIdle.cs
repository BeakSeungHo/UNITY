using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitAttackIdle : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    public MouseUnitAttackIdle(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", true);
    }

    public override void Run()
    {
        if (ownerFSM.LifeCounting())
            return;

        if (ownerFSM.IsMove)
        {
            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.RUN);
            return;
        }

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1f / ownerFSM.Base.AttackSpeed)
        {

            if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.activeSelf)
                ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
            else
                ownerFSM.ChangeFSM(MouseUnitFSM.STATE.IDLE);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", false);
    }
}
