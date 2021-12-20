using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonUnitIdle : FSM<ChameleonUnitFSM>
{
    private ChameleonUnitFSM ownerFSM;

    public ChameleonUnitIdle(ChameleonUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = ChameleonUnitFSM.STATE.IDLE;
        //ownerFSM.Animator.SetBool("Idle", true);
        //ownerFSM.hideState = true;
        //Debug.Log("Chameleon Idle Begin");
    }
    
    public override void Run()
    {
        ownerFSM.Hiding();
        //  이동 명령 우선
        if (ownerFSM.IsCommandMove)
        {

        }
        //  명령받은 공격 타겟이 있을 때
        else if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
            //if (ownerFSM.Check_InRangeAndFindPath())
            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
            else
                //ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
                ownerFSM.CommonFSM.CommandedTilePos = ownerFSM.Unit.UnitTileDest;
        }
        //  그냥 AttackTarget 이 있을 때
        else if (null != ownerFSM.AttackTarget)
        {
            //  AttackTarget 이 커맨더라면 
            if (CommonType.Commander == ownerFSM.AttackTarget.Base.Type)
            {
                //  다시 탐색.
                if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
                {
                    if (ownerFSM.Check_InRange_UsingVectorField())
                        ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
                }
            }
            //  커맨더는 아니지만 공격 범위에 있을 경우.
            else if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
                return;
            }
            //  공격 범위 안에 없으면 주변 유닛 탐색.
            else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                if (ownerFSM.Check_InRange_UsingVectorField())
                    ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
            }
        }
        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            //if (ownerFSM.Check_InRangeAndFindPath())
            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
        }
        //  커맨더가 명령한 위치와 현재 목적지가 다른 경우
        else if (ownerFSM.CommonFSM.CommandedTilePos != ownerFSM.Unit.UnitTileDest)
        {
            var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Base.MyCamp, ownerFSM.CommonFSM.CommandedTilePos, ownerFSM.Base.PlaceType);

            if (Global.InvalidTilePos != tilePos)
            {
                ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));
            }
        }

        //if (ownerFSM.IsMove)
        if (ownerFSM.CommonFSM.VFAgent.IsMove)
            ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.RUN);

        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (ownerFSM.CheckTargetInRange())
        //    {
        //        ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.CAST);
        //        return;
        //    }
        //    else if (ownerFSM.AttackTarget.activeSelf)
        //    {
        //        //ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.UpdatePath(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.RUN);
        //        return;
        //    }
        //}
        //else
        //{

        //    ownerFSM.Scout_Enemy(3);

        //    if (ownerFSM.IsMove)
        //        ownerFSM.ChangeFSM(ChameleonUnitFSM.STATE.RUN);
        //    else if (null != ownerFSM.curTile)
        //    {
        //        //ownerFSM.Command_Move(ownerFSM.MoveDest);
        //        //ownerFSM.UpdatePath(TilemapSystem.Instance.CellToWorldPos(ownerFSM.MoveDest));
        //        //ownerFSM.IsMove = true;
        //    }

        //    if (ownerFSM.CommonFSM.UnitTileDest != ownerFSM.MoveDest)
        //    {
        //        ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(ownerFSM.CommonFSM.UnitTileDest));
        //    }
        //}
    }

    public override void Exit()
    {
        ownerFSM.preState = ChameleonUnitFSM.STATE.IDLE;
        //ownerFSM.Animator.SetBool("Idle", false);
    }
}
