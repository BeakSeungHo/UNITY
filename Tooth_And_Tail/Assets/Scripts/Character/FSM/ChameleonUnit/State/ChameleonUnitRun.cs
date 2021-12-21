using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitRun : FSM<ChameleonUnitFSM>
{
    private ChameleonUnitFSM ownerFSM;

    public ChameleonUnitRun(ChameleonUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = ChameleonUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", true);
        //Debug.Log("Chameleon Run Begin");
    }

    public override void Run()
    {
        ownerFSM.Hiding();

        if (ownerFSM.IsCommandMove)
        {
            ownerFSM.AttackTarget = null;
        }
        else
        {
            if (null != ownerFSM.CommandedTarget)
            {
                ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
                ownerFSM.CommandedTarget = null;

                ownerFSM.Check_InRange_UsingVectorField();
            }

            if (null != ownerFSM.AttackTarget)
            {
                if (!ownerFSM.CheckTargetInRange())
                {
                    ownerFSM.AttackTarget = null;
                }
            }
            else if (null == ownerFSM.AttackTarget &&
                ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                //if (ownerFSM.Check_InRangeAndFindPath())
                if (ownerFSM.Check_InRange_UsingVectorField())
                {
                    //ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
                    //if (null != ownerFSM.path)
                    //    ownerFSM.path.Clear();
                }
            }
        }

        //ownerFSM.PathMove();
        //if (!ownerFSM.IsMove)
        ownerFSM.CommonFSM.VFAgent.Move();
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.IDLE);
            return;
        }

        //if (!ownerFSM.IsCommandMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //    {
        //        if (ownerFSM.Scout_Enemy(3))
        //        {
        //            if (ownerFSM.Check_EnemyInRange())
        //            {
        //                ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
        //                ownerFSM.IsMove = false;
        //                return;
        //            }
        //            else
        //            {
        //                ownerFSM.UpdatePath(ownerFSM.AttackTarget.transform.position);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.IDLE);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.Unit.gameObject.transform.position = TilemapSystem.Instance.CellToWorldPos(ownerFSM.MoveDest);
        //    ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.IDLE);
        //    return;
        //}


        ////ownerFSM.Unit.Move(ownerFSM.MoveDir);
        ////Debug.Log("Chameleon Run");
        //ownerFSM.PathMove();
    }

    public override void Exit()
    {
        ownerFSM.preState = ChameleonUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", false);
    }
}
