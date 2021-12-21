using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAIAttack : FSM<CommanderAI>
{
    private CommanderAI ownerAI = null;

    public enum AI_State { Idle, Move, Command }

    private AI_State curState = AI_State.Idle;

    private float commandTime = 0f;
    private float commandPressTime = 0.5f;

    private float unitStateCheckCount = 0f;
    private float unitStateCheckTime = 2f;

    public CommanderAIAttack(CommanderAI ownerAI)
    {
        this.ownerAI = ownerAI;
    }

    public override void Begin()
    {
        ownerAI.curGoal = CommanderAI.AI_Goal.Attack;
        curState = AI_State.Idle;
        commandTime = 0f;
        unitStateCheckCount = 0f;
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
            case AI_State.Command:
                Command();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        commandTime = 0f;

        //  적 빌딩 위치 체크.
        var curTilePos = TilemapSystem.Instance.WorldToCellPos(ownerAI.Pos);
        foreach (var tilePos in ownerAI.EnemyBuildingTile)
        {
            var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
            var node = TilemapSystem.Instance.GetTile(worldPos);

            if (null == node.occupier)
            {
                ownerAI.EnemyBuildingTile.Remove(tilePos);
                return;
            }

            if (Vector3Int.Distance(tilePos, curTilePos) < 3)
            {
                curState = AI_State.Command;
                return;
            }

            var attackTilePos = InGameManager.Instance.Find_NearestEmptyTile(worldPos);

            if (Global.InvalidTilePos == attackTilePos)
            {
                Debug.Log("CommanderAIAttack : attackTilePos is invalid");
                continue;
            }

            if (ownerAI.Find_Path(attackTilePos))
            {
                curState = AI_State.Move;
            }
            else
            {
                Debug.Log("CommanderAIAttack : attackTilePos is invalid");
                continue;
            }
        }
    }

    private void Move()
    {
        if (CommanderFSM.STATE.DEATH == ownerAI.commander.commanderFSM.curState)
        {
            curState = AI_State.Idle;
            return;
        }

        if (ownerAI.Path_Move())
        {
            curState = AI_State.Command;
            commandTime = 0f;
        }
    }

    private void Command()
    {
        commandTime += Time.deltaTime;
        if (commandTime >= commandPressTime)
        {
            //  명령 중지
            ownerAI.commandMoveAllButton = false;

            //  적빌딩이 시야안에 없으면 다시 Idle 상태로
            if (!ownerAI.Check_EnemyBuildingTile_InRange())
            {
                curState = AI_State.Idle;
            }

            //  유닛이 전부 가만히 있는지 체크.
            unitStateCheckCount += Time.deltaTime;
            if (unitStateCheckCount >= unitStateCheckTime)
            {
                unitStateCheckCount = 0f;

                if (ownerAI.Check_AllUnitState_Idle())
                {
                    curState = AI_State.Idle;
                }
            }
        }
        else
        {
            ownerAI.commandMoveAllButton = true;
        }
    }

    public override void Exit()
    {
        ownerAI.preGoal = CommanderAI.AI_Goal.Attack;
    }
}
