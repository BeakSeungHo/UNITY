using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAIDie : FSM<CommanderAI>
{
    private CommanderAI ownerAI = null;

    public CommanderAIDie(CommanderAI ownerAI)
    {
        this.ownerAI = ownerAI;
    }

    public override void Begin()
    {
        ownerAI.curGoal = CommanderAI.AI_Goal.Farming;

        ownerAI.Reset();
    }

    public override void Run()
    {

    }

    public override void Exit()
    {
        ownerAI.preGoal = CommanderAI.AI_Goal.Die;
    }

}
