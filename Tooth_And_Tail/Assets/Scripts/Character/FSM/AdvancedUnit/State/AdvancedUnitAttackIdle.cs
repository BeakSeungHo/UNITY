using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitAttackIdle : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    public AdvancedUnitAttackIdle(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", true);
        Debug.Log("AttackIdle Begin");
    }
    
    public override void Run()
    {
        switch (ownerFSM.Base.Type)
        {
            case CommonType.Boar:
                Action_Boar();
                break;
            case CommonType.Badger:
                Action_Badger();
                break;
            case CommonType.Wolf:
                Action_Wolf();
                break;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = AdvancedUnitFSM.STATE.ATTACK_IDLE;
        ownerFSM.Animator.SetBool("AttackIdle", false);
    }

    public void Action_Boar()
    {

    }

    public void Action_Badger()
    {

    }

    public void Action_Wolf()
    {
        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.TimeCount >= 1f / ownerFSM.Base.AttackSpeed)
        {
            ownerFSM.isFired = false;
            if (null == ownerFSM.AttackTarget ||
                !ownerFSM.AttackTarget.gameObject.activeSelf)
            {
                ownerFSM.AttackTarget = null;
                if (ownerFSM.IsMove)
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                else
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                return;
            }

            Character target = ownerFSM.AttackTarget.GetComponent<Character>();
            if (null == target || target.BuffDebuff.Stim)
            {
                if (ownerFSM.IsMove)
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                else
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                ownerFSM.AttackTarget = null;
                return;
            }

            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        }
    }
}
