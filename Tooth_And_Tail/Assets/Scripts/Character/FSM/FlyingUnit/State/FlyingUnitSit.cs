using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitSit : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    public FlyingUnitSit(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.SIT;

    }

    public override void Run()
    {

    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.SIT;
    }
}
