using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitSpawn : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    public MouseUnitSpawn(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.SPAWN;
        ownerFSM.Animator.SetBool("Spawn", true);
    }

    public override void Run()
    {

        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.IDLE);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.SPAWN;
        ownerFSM.Animator.SetBool("Spawn", false);
    }
}
