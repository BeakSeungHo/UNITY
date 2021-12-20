using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitDeath : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    private int preStateHash = 0;

    public FlyingUnitDeath(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", true);
        preStateHash = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        
        //  소리
        ownerFSM.Play_Unit_PositionSound(UnitSoundType.Death);
    }

    public override void Run()
    {
        switch (ownerFSM.Unit.Base.Type)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
                ownerFSM.Unit.isDead = true;
                ownerFSM.Death();
                break;
            case CommonType.Owl:
                {
                    var stateInfo = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0);

                    //  아직 애니메이션이 바뀌지 않음.
                    if (stateInfo.fullPathHash == preStateHash)
                        return;

                    if (stateInfo.normalizedTime >= 0.8f)
                    {
                        ownerFSM.Unit.isDead = true;
                        ownerFSM.Death();
                    }
                }
                break;
        }

    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", false);
    }
}
