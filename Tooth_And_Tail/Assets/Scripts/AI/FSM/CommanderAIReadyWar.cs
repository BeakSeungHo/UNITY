using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAIReadyWar : FSM<CommanderAI>
{
    private CommanderAI ownerAI = null;

    public enum AI_State { Idle, Move, Build }
    public AI_State curState = AI_State.Idle;

    private bool changeSelectedUnit = false;

    public CommanderAIReadyWar(CommanderAI ownerAI)
    {
        this.ownerAI = ownerAI;
    }

    public override void Begin()
    {
        ownerAI.curGoal = CommanderAI.AI_Goal.Ready_War;
        curState = AI_State.Idle;
        changeSelectedUnit = false;
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
        if (!changeSelectedUnit)
        {
            ownerAI.Change_SelectedUnitRandom_ByPlayTime();
            changeSelectedUnit = true;
        }

        int buildCost = SceneStarter.Instance.GetData(ownerAI.commander.SelectedUnit).BuildCost;
        if (buildCost <= ownerAI.Food)
        {
            var worldPos = ownerAI.Get_WorldPosForBuild();

            if (Global.InvalidWorldPos == worldPos)
            {
                Debug.Log("CommanderAIReadyWar : worldPos is invalid");
                return;
            }

            if (ownerAI.Find_Path(worldPos))
                curState = AI_State.Move;
            else
            {
                Debug.Log("CommanderAIReadyWar : Find_Path fail");
                return;
            }
        }

    }

    private void Move()
    {
        if (ownerAI.Path_Move())
            curState = AI_State.Build;
    }

    private void Build()
    {
        ownerAI.buildButton = true;
        curState = AI_State.Idle;
        changeSelectedUnit = false;
    }

    public override void Exit()
    {
        ownerAI.preGoal = CommanderAI.AI_Goal.Ready_War;
    }

}
