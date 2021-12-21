using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitIdle : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    public FlyingUnitIdle(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.IDLE;
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
        ownerFSM.preState = FlyingUnitFSM.STATE.IDLE;
    }

    /// <summary>
    /// Pigeon 의 행동
    /// </summary>
    private void Action_Pigeon()
    {

        //  그냥 AttackTarget 이 있을 때
        //  걔가 범위 내에 있다면
        if (null != ownerFSM.AttackTarget && ownerFSM.CheckTargetInRange())
        {
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
            return;
        }
        //  없을 때 주변에 치료가 필요한 유닛을 찾는다
        else if (ownerFSM.Scout_FriendUnit())
        {
            if (ownerFSM.Check_InRangeAndSetMoveDir())
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
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

        if (ownerFSM.IsMove)
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);






        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (ownerFSM.CheckTargetInRange())
        //    {
        //        ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
        //        return;
        //    }

        //    if (!ownerFSM.AttackTarget.activeSelf)
        //        ownerFSM.AttackTarget = null;
        //    else
        //    {
        //        ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
        //    }
        //    return;
        //}
        //else
        //{
        //    //  아군 범위 안의 아군 유닛중 피가 풀피 아닌 유닛을 찾는다.
        //    ownerFSM.Scout_FriendUnit();

        //    if (ownerFSM.IsMove)
        //        ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
            
        //}



    }

    /// <summary>
    /// Falcon 의 행동
    /// </summary>

    private void Action_Falcon()
    {
        //  이동 명령 우선
        if (ownerFSM.IsCommandMove)
        {

        }
        //  명령받은 공격 타겟이 있을 때
        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
            if (ownerFSM.Check_InRangeAndSetMoveDir())
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
            else
            {
                //ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
                ownerFSM.CommonFSM.CommandedTilePos = ownerFSM.Unit.UnitTileDest;
            }
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
                        ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
                }
            }
            //  커맨더는 아니지만 공격 범위에 있을 경우.
            else if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
                return;
            }
            //  공격 범위 안에 없으면 주변 유닛 탐색.
            else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                if (ownerFSM.Check_InRange_UsingVectorField())
                    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
            }
        }
        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            if (ownerFSM.Check_InRangeAndSetMoveDir())
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.CAST);
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

        if (ownerFSM.IsMove)
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);

        
    }

    /// <summary>
    /// Owl 의 행동.
    /// </summary>

    private void Action_Owl()
    {
        ownerFSM.Owl_SpawnMouse();

        if (ownerFSM.IsMove)
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
    }
}
