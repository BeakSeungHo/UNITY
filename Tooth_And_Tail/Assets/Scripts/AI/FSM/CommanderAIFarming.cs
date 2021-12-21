using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAIFarming : FSM<CommanderAI>
{
    private CommanderAI ownerAI = null;

    public enum AI_State { Idle, Move, Build, Rally }
    
    private enum AI_Action { Build, Rally }

    private AI_State curState = AI_State.Idle;
    private AI_Action curAction = AI_Action.Build;

    private float rallyTimeCount = 0f;
    private float rallyTimeMax = 0.5f;

    private Vector3 campFirePos = Vector3.zero;

    public CommanderAIFarming(CommanderAI ownerAI)
    {
        this.ownerAI = ownerAI;
    }

    public override void Begin()
    {
        ownerAI.curGoal = CommanderAI.AI_Goal.Farming;
        curState = AI_State.Idle;
        rallyTimeCount = 0f;
    }

    public override void Run()
    {
        switch (curState)
        {
            case AI_State.Idle:
                Idle();
                break;
            case AI_State.Move:
                Move();
                break;
            case AI_State.Build:
                Build();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        if (ownerAI.Food >= 60)
        {
            //  현재 짓고 있는 제분소가 있으면 대기
            if (ownerAI.Check_GristmillUnderConstruction())
                return;

            //  농장지을 위치 찾기
            var farmTilePos = ownerAI.Get_FirstFarmTilePos();

            //  그 위치가 없음
            if (Global.InvalidTilePos == farmTilePos)
            {
                //  맵 전체에 남은 제분소가 없으면
                if (0 == ownerAI.RemainedWorldGristmill)
                {
                    //  캠프파이어 지을 수 있는 장소가 있는지 확인.
                    if (ownerAI.Check_CampFire(out campFirePos))
                    {
                        curAction = AI_Action.Build;
                        if (!Go(campFirePos))
                            return;
                    }
                    //  유닛수가 충분하면 카빈 위치 확인
                    else if (ownerAI.UnitCount >= 6 && ownerAI.Check_Cabin(out campFirePos))
                    {
                        curAction = AI_Action.Rally;
                        if (!Go(campFirePos))
                            return;
                    }
                    //  없으면 농사 못지음,
                    else
                    {
                        ownerAI.CanFarming = false;
                        return;
                    }
                }

                //  제분소 짓기 위한 위치 찾기
                var gristmillWorldPos = ownerAI.Get_WorldPosForBuildGristmill();

                //  못찾음
                if (Global.InvalidWorldPos == gristmillWorldPos)
                {
                    Debug.Log("CommanderAIFarming : gristmillWorldPos is invalid");
                    return;
                }

                curAction = AI_Action.Build;
                if (!Go(gristmillWorldPos))
                    return;
            }
            else
            {
                curAction = AI_Action.Build;
                //  농장 위치로 이동
                if (ownerAI.Find_Path(farmTilePos))
                    curState = AI_State.Move;
                else
                {
                    Debug.Log("CommanderAIFarming : Find_Path fail");
                    return;
                }
            }
        }
    }

    private void Move()
    {
        if (ownerAI.Path_Move())
        {
            switch (curAction)
            {
                case AI_Action.Build:
                    curState = AI_State.Build;
                    break;
                case AI_Action.Rally:
                    curState = AI_State.Rally;
                    break;
            }
        }
    }

    private void Build()
    {
        ownerAI.buildButton = true;
        curState = AI_State.Idle;
    }
    
    private void Rally()
    { 
        //  카빈을 공격하기에는 너무 유닛이 적으면 안됌.
        if (ownerAI.UnitCount < 6)
        {
            curState = AI_State.Idle;
            return;
        }

        //  일정 시간동안 Rally 버튼 누름.
        if (rallyTimeCount <= rallyTimeMax)
            ownerAI.commandMoveAllButton = true;

        rallyTimeCount += Time.deltaTime;

        //  모든 유닛이 가만히 잇으면 다시 누름.
        if (ownerAI.Check_AllUnitState_Idle())
        {
            rallyTimeCount = 0f;
        }

        if (Global.InvalidWorldPos == campFirePos)
        {
            Debug.Log("campFirePos is invalid");
            curState = AI_State.Idle;
            return;
        }

        var node = TilemapSystem.Instance.GetTile(campFirePos);
        if (null == node?.occupier)
        {
            Debug.Log("node?.occupier is null");
            curState = AI_State.Idle;
            return;
        }

        if (CommonType.CampFire == node.occupier.Base.Type )
        {
            if (Camp.End == node.occupier.Base.MyCamp)
            {
                curState = AI_State.Build;
                return;
            }
            else
            {
                curState = AI_State.Idle;
                return;
            }
        }

    }

    //  해당 위치로 이동.
    private bool Go(Vector3 worldPosition)
    {
        var tilePos = InGameManager.Instance.Find_NearestEmptyTile(worldPosition);

        if (Global.InvalidTilePos == tilePos)
        {
            Debug.Log("tilePos is invalid");
            return false;
        }

        //  찾은 타일로 가는 경로 찾기
        if (ownerAI.Find_Path(tilePos))
            curState = AI_State.Move;

        //  못찾음
        else
        {
            Debug.Log("Find_Path fail");
            return false;
        }

        return true;
    }

    public override void Exit()
    {
        ownerAI.preGoal = CommanderAI.AI_Goal.Farming;
    }

}
