using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitIdle : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    public AdvancedUnitIdle(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", true);
        ownerFSM.AttackEffect = false;
    }
    
    public override void Run()
    {
        switch (ownerFSM.Base.Type)
        {
            case CommonType.Boar:
                Action_Boar();
                break;
            case CommonType.Badger:
                Action_Badger();
                break;
            case CommonType.Wolf:
                Action_Wolf();
                break;
        }

    }

    public override void Exit()
    {
        ownerFSM.preState = AdvancedUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Idle", false);
    }

    public void Action_Boar()
    {
        //  명령받은 공격 타겟이 있을 때
        if (null != ownerFSM.CommandedTarget)
        {
            var AttackTarget = ownerFSM.CommandedTarget;
            
            ownerFSM.CommandedTarget = null;

            var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, 
                                                                       AttackTarget.transform.position);

            var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

            ownerFSM.Command_Move(node.worldPosition);
            ownerFSM.CommonFSM.CommandedTilePos = tilePos;

            //ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
            //return;
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
                        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
                }
            }
            else
            {
                if (ownerFSM.CheckTargetInRange())
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
                else
                    ownerFSM.AttackTarget = null;
                return;
            }
        }
        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            if (ownerFSM.CheckTargetInRange())
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
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
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);


        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (!ownerFSM.AttackTarget.activeSelf)
        //    {
        //        ownerFSM.AttackTarget = null;
        //        return;
        //    }

        //    if (ownerFSM.CheckTargetInRange())
        //    {
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //        return;
        //    }
        //    else
        //    {
        //        ownerFSM.UpdatePath(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //    }
        //}
        //else
        //{
        //    ownerFSM.Scout_Enemy();

        //    if (ownerFSM.IsMove)
        //    {
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //        return;
        //    }

        //    if (ownerFSM.CommonFSM.UnitTileDest != ownerFSM.MoveDest)
        //    {
        //        ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(ownerFSM.CommonFSM.UnitTileDest));
        //    }
        //}
    }

    public void Action_Badger()
    {
        ownerFSM.Badger_Cooling();

        if (null != ownerFSM.CommandedTarget)
        {
            var AttackTarget = ownerFSM.CommandedTarget;

            ownerFSM.CommandedTarget = null;

            var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp,
                                                                       AttackTarget.transform.position);

            var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

            ownerFSM.Command_Move(node.worldPosition);

            ownerFSM.CommonFSM.CommandedTilePos = tilePos;

            //ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
            //return;
        }
        //  그냥 AttackTarget 이 있을 때
        else if (null != ownerFSM.AttackTarget )
        {
            //  AttackTarget 이 커맨더라면 
            if (CommonType.Commander == ownerFSM.AttackTarget.Base.Type)
            {
                //  다시 탐색.
                if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
                {
                    if (ownerFSM.Check_InRange_UsingVectorField())
                        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
                }
            }
            else
            {
                if (ownerFSM.CheckTargetInRange())
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
                else
                    ownerFSM.AttackTarget = null;
                return;
            }
        }
        //  없을 때 주변에 적이 있는지 확인하고 잇으면 공격
        else if (ownerFSM.Scout_Enemy(Mathf.Max(ownerFSM.Base.Range, ownerFSM.Base.Sight)))
        {
            if (ownerFSM.CheckTargetInRange())
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
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
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);


        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (ownerFSM.CheckTargetInRange())
        //    {
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //        return;
        //    }
        //    if (!ownerFSM.AttackTarget.activeSelf)
        //        ownerFSM.AttackTarget = null;
        //    else
        //    {
        //        ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //    }
        //    return;
        //}
        //else
        //{
        //    ownerFSM.Scout_Enemy();

        //    if (ownerFSM.IsMove)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //    else if (null != ownerFSM.curTile)
        //    {
        //        //ownerFSM.IsMove = true;
        //    }

        //    if (ownerFSM.CommonFSM.UnitTileDest != ownerFSM.MoveDest)
        //    {
        //        ownerFSM.Command_Move(TilemapSystem.Instance.CellToWorldPos(ownerFSM.CommonFSM.UnitTileDest));
        //    }
        //}
    }

    public void Action_Wolf()
    {
        //if (null != ownerFSM.CommandedTarget)
        //{
        //    var attackTarget = ownerFSM.CommandedTarget;
        //    ownerFSM.CommandedTarget = null;
        //    var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp,
        //                                                    attackTarget.transform.position);

        //    var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

        //    ownerFSM.Command_Move(node.worldPosition);
        //    return;
        //}

        if (ownerFSM.IsCommandMove)
        {
            if (ownerFSM.CommonFSM.VFAgent.IsMove)
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                return;
            }

        }
        if (null != ownerFSM.CommandedTarget)
        {
            var worldPos = ownerFSM.CommandedTarget.transform.position;

            ownerFSM.CommandedTarget = null;

            var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, worldPos);

            var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

            ownerFSM.Command_Move(node.worldPosition);

            ownerFSM.CommonFSM.CommandedTilePos = tilePos;
        }
        else if (null != ownerFSM.AttackTarget)
        {
            if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
                return;
            }

            ownerFSM.Command_Move(ownerFSM.AttackTarget.transform.position);
        }
        else if (ownerFSM.Scout_FriendUnit())
        {
            if (ownerFSM.CheckTargetInRange())
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
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
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);

    }
}
