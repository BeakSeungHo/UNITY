using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderDig : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM = null;


    public CommanderDig(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.DIG;
        ownerFSM.commander.digTimeCount = 0f;
        ownerFSM.animator.SetBool("Dig", true);

        ownerFSM.Play_CommanderSound(Sound_Channel.Effect, ComSoundType.Return, 0);
    }

    public override void Run()
    {
        ownerFSM.commander.digTimeCount += Time.deltaTime;

        if (ownerFSM.commander.digTimeCount > ownerFSM.commander.digTime)
        {
            ownerFSM.ChangeFSM(CommanderFSM.STATE.RETURN);
            return;
        }

        if (!ownerFSM.IsDigging)
            ownerFSM.ChangeFSM(CommanderFSM.STATE.IDLE);
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.DIG;
        ownerFSM.animator.SetBool("Dig", false);
    }
}
