using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitAttackIdle : FSM<NormalUnitFSM>
{
    private NormalUnitFSM ownerFSM;

    public NormalUnitAttackIdle(NormalUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = NormalUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", true);
        //Debug.Log("NormalUnitAttackIdle begin");

        if (CommonType.Fox == ownerFSM.Base.Type)
            ownerFSM.Play_Unit_Sound(UnitSoundType.Reload);
    }

    public override void Run()
    {
        if (ownerFSM.IsMove)
        {
            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.RUN);
            return;
        }

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1 / ownerFSM.Base.AttackSpeed)
        {
            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.IDLE);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = NormalUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", false);
    }
}
