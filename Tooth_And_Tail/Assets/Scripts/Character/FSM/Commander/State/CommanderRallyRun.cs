using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderRallyRun : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM;

    public CommanderRallyRun(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.RALLY_RUN;
        ownerFSM.animator.SetBool("Run", true);
        ownerFSM.animator.SetBool("Rally", true);

        ownerFSM.Play_CommanderSound(ComSoundType.Attack);
    }

    public override void Run()
    {
        if (ownerFSM.InputRallyAll)
            ownerFSM.commander.Command_Move_All();
        else if (ownerFSM.InputRally)
            ownerFSM.commander.Command_Move();

        Vector3 move;

        bool isMove = ownerFSM.InputMove(out move);

        ownerFSM.commander.Move(move);

        if (ownerFSM.isRally)
        {
            if (!isMove)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_STAND);
        }
        else
            ownerFSM.ChangeFSM(isMove ? CommanderFSM.STATE.RUN : CommanderFSM.STATE.IDLE);

        ownerFSM.Play_FootStepSound();
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.RALLY_RUN;
        ownerFSM.animator.SetBool("Run", false);
        ownerFSM.animator.SetBool("Rally", false);
    }
}
