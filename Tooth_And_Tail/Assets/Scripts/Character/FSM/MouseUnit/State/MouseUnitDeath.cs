using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitDeath : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    public MouseUnitDeath(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", true);

        ownerFSM.Play_Unit_PositionSound(UnitSoundType.Death);
    }

    public override void Run()
    {
        //if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            ownerFSM.Unit.isDead = true;
            ownerFSM.Death();
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", false);
    }
}
