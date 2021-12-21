using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitCast : FSM<ChameleonUnitFSM>
{
    private ChameleonUnitFSM ownerFSM;

    private bool isFired = false;

    public ChameleonUnitCast(ChameleonUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = ChameleonUnitFSM.STATE.CAST;
        ownerFSM.TimeCount = 0f;
        ownerFSM.Animator.SetBool("Cast", true);
        ownerFSM.Animator.SetBool("Run", false);
        isFired = false;
        //ownerFSM.hideState = true;
        //Debug.Log("Chameleon Cast Begin");
    }

    public override void Run()
    {
        if (ownerFSM.hideState)
        {
            //Debug.Log("Cast Run hideState true");
            ownerFSM.hideState = false;
            ownerFSM.spriteRenderer.color = new Color(1, 1, 1, 1f);
        }

        //if (ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.RUN);
        //    return;
        //}

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.ATTACK_IDLE);
            return;
        }

        if (!isFired)
        {
            //  발사!

            if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                isFired = true;
                if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.activeSelf)
                {
                    Character character = ownerFSM.AttackTarget.GetComponent<Character>();

                    Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                    if (ownerFSM.Unit.facingRight != delta.x >= 0)
                        ownerFSM.Unit.Flip();

                    //  소리
                    ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                    if (null != character)
                    {
                        character.Hit(ownerFSM.Base.Damage, ownerFSM.Unit);
                        return;
                    }
                    else
                        Debug.Log("Chameleon Cast : AttackTarget character is null");

                }
            }
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = ChameleonUnitFSM.STATE.CAST;
        ownerFSM.Animator.SetBool("Cast", false);
    }

}
