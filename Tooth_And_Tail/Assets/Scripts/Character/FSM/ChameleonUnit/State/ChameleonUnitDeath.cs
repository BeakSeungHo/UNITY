using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitDeath : FSM<ChameleonUnitFSM>
{
    private ChameleonUnitFSM ownerFSM;

    public ChameleonUnitDeath(ChameleonUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = ChameleonUnitFSM.STATE.DEATH;
        ownerFSM.Play_Unit_PositionSound(UnitSoundType.Death);
        //Debug.Log("Chameleon Death");
    }

    public override void Run()
    {
        //if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            ownerFSM.Unit.isDead = true;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = ChameleonUnitFSM.STATE.DEATH;
    }
}
