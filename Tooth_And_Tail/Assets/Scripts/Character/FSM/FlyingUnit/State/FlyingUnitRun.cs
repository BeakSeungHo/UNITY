using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitRun : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    public FlyingUnitRun(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.RUN;
    }

    public override void Run()
    {
        switch (ownerFSM.Data.CommonType)
        {
            case CommonType.Pigeon:
                Action_Pigeon();
                break;
            case CommonType.Falcon:
                Action_Falcon();
                break;
            case CommonType.Owl:
                Action_Owl();
                break;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.RUN;
    }

    private void  Action_Pigeon()
    {
        //  이동 명령을 내리고 있는 경우
        if (ownerFSM.IsCommandMove)
        {
            ownerFSM.AttackTarget = null;

            ////  목적지에 도착했는지 확인.
            //if (ownerFSM.IsArrive())
            //{
            //    ownerFSM.IsMove = false;
            //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
            //    return;
            //}

            ////  목적지로 이동
            //ownerFSM.Unit.Move(ownerFSM.MoveDir);
            //return;
        }
        else
        {
            //  커맨더가 명령을 내린 타겟이 있음.
            if (null != ownerFSM.CommandedTarget)
            {
                //  그 타겟이 아군일 경우
                if (ownerFSM.CommandedTarget.Base.MyCamp == ownerFSM.Base.MyCamp)
                    ownerFSM.AttackTarget = ownerFSM.CommandedTarget;

                ownerFSM.CommandedTarget = null;
            }
            if (null == ownerFSM.AttackTarget &&
                ownerFSM.Scout_FriendUnit())
            {
                if (ownerFSM.Check_InRangeAndSetMoveDir())
                {
                    ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
                    if (null != ownerFSM.path)
                        ownerFSM.path.Clear();
                    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
                    return;
                }
            }

        }


        ownerFSM.Unit.Move(ownerFSM.MoveDir);

        if (!ownerFSM.IsMove || ownerFSM.IsArrive())
        {
            ownerFSM.IsMove = false;
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
            return;
        }


    }

    private void Action_Falcon()
    {
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

                ownerFSM.Check_InRangeAndSetMoveDir();
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
                if (ownerFSM.Check_InRangeAndSetMoveDir())
                {
                    ownerFSM.curTile = TilemapSystem.Instance.GetTile(ownerFSM.Pos);
                    if (null != ownerFSM.path)
                        ownerFSM.path.Clear();
                }
            }
        }

        ownerFSM.Unit.Move(ownerFSM.MoveDir);

        if (!ownerFSM.IsMove || ownerFSM.IsArrive())
        {
            ownerFSM.IsMove = false;
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
            return;
        }



        //if (!ownerFSM.IsCommandMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //    {
        //        if (ownerFSM.Scout_Enemy())
        //        {
        //            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
        //            ownerFSM.IsMove = false;
        //            return;
        //        }
        //    }
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
        //    return;
        //}

        //ownerFSM.Unit.Move(ownerFSM.MoveDir);
        //ownerFSM.PathMove();
    }

    private void Action_Owl()
    {
        ownerFSM.Owl_SpawnMouse();

        if (ownerFSM.IsArrive())
        {
            ownerFSM.IsMove = false;
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
            return;
        }
        ownerFSM.Unit.Move(ownerFSM.MoveDir);
    }

}
