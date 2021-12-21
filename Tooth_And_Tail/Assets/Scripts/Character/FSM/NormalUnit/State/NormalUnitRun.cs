using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitRun : FSM<NormalUnitFSM>
{
    private NormalUnitFSM ownerFSM;

    public NormalUnitRun(NormalUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = NormalUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", true);
        //Debug.Log("NormalUnitRun Begin");
    }

    public override void Run()
    {
        if (ownerFSM.IsCommandMove)
        {
            ownerFSM.AttackTarget = null;
        }
        else
        {
            //  지정된 공격 타겟이 있음
            if (null != ownerFSM.CommandedTarget)
            {
                ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
                ownerFSM.CommandedTarget = null;

                if (ownerFSM.Check_InRange_UsingVectorField())
                    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
                else
                    ownerFSM.CommonFSM.CommandedTilePos = ownerFSM.Unit.UnitTileDest;
            }

            if (null != ownerFSM.AttackTarget)
            {
                if (!ownerFSM.CheckTargetInRange())
                {
                    ownerFSM.AttackTarget = null;
                }
            }
            //  공격 타겟이 없으면, 주변에 적 있는지 체크.
            else if (null == ownerFSM.AttackTarget &&
                    ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                //if (ownerFSM.Check_InRangeAndFindPath())
                if (ownerFSM.Check_InRange_UsingVectorField())
                {
                    //  ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
                    //  return;
                    //  ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
                    //  if (null != ownerFSM.path)
                    //    ownerFSM.path.Clear();
                }
            }
        }

        //ownerFSM.PathMove();

        //if (!ownerFSM.IsMove)

        ownerFSM.CommonFSM.VFAgent.Move();
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.IDLE);
            return;
        }


        ////  커맨더가 이동 명령을 내리고 있을 때.
        //if (ownerFSM.IsCommandMove)
        //{
        //    ownerFSM.AttackTarget = null;
        //}
        ////  커맨더가 이동 명령을 내리고 있지 않음.
        //else
        //{
        //    //  따로 명령받은 공격 대상이 있을 때.
        //    if (null != ownerFSM.CommandedTarget)
        //    {
        //        if (ownerFSM.Check_InRangeAndFindPath(ownerFSM.CommandedTarget))
        //        {
        //            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
        //            ownerFSM.CommandedTarget = null;
        //            ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
        //            if (null != ownerFSM.path)
        //                ownerFSM.path.Clear();
        //        }
        //    }

        //    //  따로 공격 받은 대상이 있진 않으나, 공격할 대상이 정해져 있을 때.
        //    else if (null != ownerFSM.AttackTarget)
        //    {
        //        //  공격할 대상을 바로 공격할 수 있으면 true, 아니면 false를 반환하고 공격하기 위한 경로만 찾음.
        //        if (ownerFSM.Check_InRangeAndFindPath())
        //        {
        //            //  지금 바로 공격할 수 있으므로 이동 경로화
        //            //  curTile에 현재 위치 타일 노드를 넣으면 
        //            ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
        //            if (null != ownerFSM.path)
        //                ownerFSM.path.Clear();
        //        }
        //    }

        //    //  공격할 대상이 없으면 주변에 공격할 대상이 있는지 체크한다.
        //    else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        //    {
        //        if (ownerFSM.Check_InRangeAndFindPath())
        //        {
        //            ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
        //            if (null != ownerFSM.path)
        //                ownerFSM.path.Clear();
        //        }
        //    }
        //}

        //ownerFSM.PathMove();

        //if (!ownerFSM.IsMove || ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.IDLE);
        //    return;
        //}
        /////////////////////////////////////////////////////////////////////

        //if (!ownerFSM.IsCommandMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //    {
        //        if (ownerFSM.Base.Type == CommonType.Mole)
        //        {
        //            ownerFSM.Scout_Enemy(3);
        //        }
        //        else if (ownerFSM.Scout_Enemy())
        //        {
        //            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.IDLE);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.IDLE);
        //    return;
        //}

        ////ownerFSM.Unit.Move(ownerFSM.MoveDir);
        //ownerFSM.PathMove();
    }

    public override void Exit()
    {
        ownerFSM.preState = NormalUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", false);
    }
}
