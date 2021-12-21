using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitIdle : FSM<NormalUnitFSM>
{
    private NormalUnitFSM ownerFSM;

    public NormalUnitIdle(NormalUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = NormalUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", true);
    }

    public override void Run()
    {
        //  이동 명령 우선
        if (ownerFSM.IsCommandMove)
        {

        }
        //  명령받은 공격 타겟이 있을 때
        else if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;

            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
            else
                ownerFSM.CommonFSM.CommandedTilePos = ownerFSM.Unit.UnitTileDest;
        }
        //  그냥 AttackTarget 이 있을 때
        //  AttackTarget 이 커맨더라면 
        else if (null != ownerFSM.AttackTarget && CommonType.Commander == ownerFSM.AttackTarget.Base.Type)
        {
            //  다시 탐색.
            if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
            {
                if (ownerFSM.Check_InRange_UsingVectorField())
                    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
            }
        }
        //  커맨더는 아니지만 공격 범위에 있을 경우.
        else if (null != ownerFSM.AttackTarget && ownerFSM.CheckTargetInRange())
        {
            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
            return;
        }

        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            if (ownerFSM.Check_InRange_UsingVectorField())
                ownerFSM.ChangeFSM(NormalUnitFSM.STATE.CAST);
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
        

        if (ownerFSM.CommonFSM.VFAgent.IsMove)
            ownerFSM.ChangeFSM(NormalUnitFSM.STATE.RUN);

    }

    public override void Exit()
    {
        ownerFSM.preState = NormalUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", false);
    }
}
