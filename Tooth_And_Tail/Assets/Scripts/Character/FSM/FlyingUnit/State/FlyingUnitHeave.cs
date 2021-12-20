using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitHeave : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    public FlyingUnitHeave(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.HEAVE;

    }

    public override void Run()
    {

    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.HEAVE;
    }
}
