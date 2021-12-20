using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderRallyStand : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM;

    public CommanderRallyStand(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.RALLY_STAND;
        ownerFSM.animator.SetBool("Run", false);
        ownerFSM.animator.SetBool("Rally", true);

        ownerFSM.Play_CommanderSound(ComSoundType.Attack);
    }

    public override void Run()
    {
       // Debug.Log("CommanderRallyStand : Run");
        if (ownerFSM.InputRallyAll)
        {
            //Debug.Log("This is Right!");
            ownerFSM.commander.Command_Move_All();
        }
        else if(ownerFSM.InputRally)
        {
            //Debug.Log("This is Error!");
            ownerFSM.commander.Command_Move();
        }

        Vector3 move;

        bool isMove = ownerFSM.InputMove(out move);

        ownerFSM.commander.Move(move);

        if (ownerFSM.isRally)
        {
            if (isMove)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_RUN);
        }
        else
            ownerFSM.ChangeFSM(isMove ? CommanderFSM.STATE.RUN : CommanderFSM.STATE.IDLE);
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.RALLY_STAND;
        ownerFSM.animator.SetBool("Rally", false);

    }
}
