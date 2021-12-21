using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderIdle : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM = null;
    
    public CommanderIdle(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }
    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.IDLE;

        ownerFSM.animator.SetBool("Idle", true);
        ownerFSM.CanBeTarget = true;
    }

    public override void Run()
    {
        if (ownerFSM.InputDigging)
        {
            ownerFSM.ChangeFSM(CommanderFSM.STATE.DIG);
            return;
        }

        Vector3 move;

        bool isMove = ownerFSM.InputMove(out move);

        if (isMove)
        {
            ownerFSM.commander.Move(move);

            if (ownerFSM.isRally)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_RUN);
            else if (ownerFSM.isAttack)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.ATTACK_RUN);
            else
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RUN);
        }
        else
        {
            if (ownerFSM.isRally)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_STAND);
            else if (ownerFSM.isAttack)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.ATTACK_STAND);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.IDLE;
        ownerFSM.animator.SetBool("Idle", false);
    }

}
