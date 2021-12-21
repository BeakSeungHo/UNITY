using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAIScout : FSM<CommanderAI>
{
    private CommanderAI ownerAI = null;

    public enum AI_State { Idle, Move }

    private AI_State curState = AI_State.Idle;

    public CommanderAIScout(CommanderAI ownerAI)
    {
        this.ownerAI = ownerAI;
    }

    public override void Begin()
    {
        ownerAI.curGoal = CommanderAI.AI_Goal.Scout;
        curState = AI_State.Idle;
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
        }
    }

    private void Idle()
    {
        if (ownerAI.Find_PathToGristmills())
            curState = AI_State.Move;
    }

    private void Move()
    {
        if (ownerAI.Path_Move())
            curState = AI_State.Idle;
        else
            ownerAI.Scout_EnemyBuilding();
    }

    public override void Exit()
    {
        ownerAI.preGoal = CommanderAI.AI_Goal.Scout;
    }
}
