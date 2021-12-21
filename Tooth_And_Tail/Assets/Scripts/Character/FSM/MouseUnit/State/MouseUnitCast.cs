using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitCast : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    private bool isFired = false;

    public MouseUnitCast(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.IDLE;
        ownerFSM.TimeCount = 0f;
        ownerFSM.Animator.SetBool("Cast", true);
        isFired = false;
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

        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.ATTACK_IDLE);
        }

        if (!isFired)
        {
            if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                isFired = true;
                if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.activeSelf)
                {
                    //  소리
                    ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                    //  데미지
                    Character character = ownerFSM.AttackTarget.GetComponent<Character>();

                    if (null != character)
                    {
                        character.Hit(ownerFSM.Base.Damage, ownerFSM.Unit);
                        return;
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Cast", false);
    }
}
