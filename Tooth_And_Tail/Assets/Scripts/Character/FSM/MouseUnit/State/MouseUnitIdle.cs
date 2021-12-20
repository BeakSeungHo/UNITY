using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUnitIdle : FSM<MouseUnitFSM>
{
    private MouseUnitFSM ownerFSM;

    public MouseUnitIdle(MouseUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = MouseUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", true);
    }

    public override void Run()
    {
        if (ownerFSM.LifeCounting())
            return;
        
        //  명령받은 공격 타겟이 있을 때
        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
            //if (ownerFSM.Check_InRangeAndFindPath())
            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
            else
                //ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
                ownerFSM.CommonFSM.CommandedTilePos = ownerFSM.Unit.UnitTileDest;
        }
        //  그냥 AttackTarget 이 있을 때
        //  걔가 범위 내에 있다면
        else if (null != ownerFSM.AttackTarget)
        {
            //  AttackTarget 이 커맨더라면 
            if (CommonType.Commander == ownerFSM.AttackTarget.Base.Type)
            {
                //  다시 탐색.
                if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight + 2)))
                {
                    if (ownerFSM.Check_InRange_UsingVectorField())
                        ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
                }
            }
            //  커맨더는 아니지만 공격 범위에 있을 경우.
            else if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
                return;
            }
            //  공격 범위 안에 없으면 주변 유닛 탐색.
            else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                if (ownerFSM.Check_InRange_UsingVectorField())
                    ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
            }
        }
        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            //if (ownerFSM.Check_InRangeAndFindPath())
            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
        }
        //  자신이 쫒은 부엉이 위치와 내 위치가 다를경우.
        else
        {
            var owlTilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Owl.transform.position);
            
            if (owlTilePos != ownerFSM.Unit.UnitTileDest)
            {
                //var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Base.MyCamp, ownerFSM.CommonFSM.CommandedTilePos, ownerFSM.Base.PlaceType);

                if (Global.InvalidTilePos != owlTilePos)
                {
                    //var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(owlTilePos));
                    //ownerFSM.Command_Move(node.worldPosition);
                    if (!TilemapSystem.Instance.IsInBoundsTile(owlTilePos))
                    {
                        Debug.Log("MouseUnitIdle - owlTilePos : " + owlTilePos);
                    }
                    else
                        ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(owlTilePos));
                }
            }

        }

        //if (ownerFSM.IsMove)
        if (ownerFSM.CommonFSM.VFAgent.IsMove)
            ownerFSM.ChangeFSM(MouseUnitFSM.STATE.RUN);


        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (ownerFSM.CheckTargetInRange())
        //    {
        //        ownerFSM.ChangeFSM(MouseUnitFSM.STATE.CAST);
        //        return;
        //    }
        //    else if (ownerFSM.AttackTarget.activeSelf)
        //    {
        //        ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.ChangeFSM(MouseUnitFSM.STATE.RUN);
        //        return;
        //    }
        //}
        //else
        //{
        //    ownerFSM.Scout_Enemy(5);

        //    if (ownerFSM.IsMove)
        //        ownerFSM.ChangeFSM(MouseUnitFSM.STATE.RUN);

        //    else if (null != ownerFSM.curTile)
        //    {
        //        //ownerFSM.Command_Move(ownerFSM.MoveDest);
        //        ownerFSM.IsMove = true;
        //    }

        //    Vector3Int tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Owl.transform.position);

        //    if (ownerFSM.CommonFSM.UnitTileDest != ownerFSM.MoveDest || tilePos != ownerFSM.MoveDest)
        //    {
        //        ownerFSM.Unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));
        //    }


        //    //if (ownerFSM.CommonFSM.UnitTileDest != ownerFSM.MoveDest)
        //    //{
        //    //    ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(ownerFSM.MoveDest));
        //    //}

    }

    public override void Exit()
    {
        ownerFSM.preState = MouseUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", false);
    }
}
