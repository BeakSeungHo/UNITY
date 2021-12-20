using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderDeath : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM = null;

    bool frameLatency = false;
    public CommanderDeath(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.DEATH;
        ownerFSM.animator.SetBool("Death", true);
        frameLatency = false;
        ownerFSM.CanBeTarget    = false;

        ownerFSM.Play_CommanderSound(ComSoundType.Death);
    }

    public override void Run()
    {
        if (!frameLatency)
        {
            frameLatency = true;
            return;
        }
        var stateInfo = ownerFSM.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 0.8f)
        {
            ownerFSM.ChangeFSM(CommanderFSM.STATE.RESPAWN);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.DEATH;
        ownerFSM.animator.SetBool("Death", false);
    }
}
