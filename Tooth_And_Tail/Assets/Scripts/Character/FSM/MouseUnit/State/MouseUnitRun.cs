using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitRun : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    public MouseUnitRun(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", true);
    }

    public override void Run()
    {
        if (ownerFSM.LifeCounting())
            return;

        if (null == ownerFSM.AttackTarget &&
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

        //ownerFSM.PathMove();

        //if (!ownerFSM.IsMove)

        ownerFSM.CommonFSM.VFAgent.Move();
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.IDLE);
            return;
        }


        //if (!ownerFSM.IsCommandMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //    {
        //        if (ownerFSM.Check_EnemyInRange())
        //        {
        //            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(MouseUnitFSM.STATE.IDLE);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(MouseUnitFSM.STATE.IDLE);
        //    return;
        //}

        ////ownerFSM.Unit.Move(ownerFSM.MoveDir);
        //ownerFSM.PathMove();
    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", false);
    }
}
