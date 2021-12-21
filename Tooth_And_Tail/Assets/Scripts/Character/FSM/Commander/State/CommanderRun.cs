using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderRun : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM;
    
    public CommanderRun(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
        //commander = owner.GetComponent<Commander>();
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.RUN;
        
        ownerFSM.animator.SetBool("Run", true);
    }

    public override void Run()
    {
        Vector3 move;

        bool isMove  = ownerFSM.InputMove(out move);
        bool isRally = ownerFSM.InputRally;

        if (isMove)
        {
            ownerFSM.commander.Move(move);
            if (ownerFSM.isRally)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_RUN);
            else if (ownerFSM.isAttack)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.ATTACK_RUN);
        }
        else
        {
            if (ownerFSM.isRally)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.RALLY_STAND);
            else if (ownerFSM.isAttack)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.ATTACK_STAND);
            else
                ownerFSM.ChangeFSM(CommanderFSM.STATE.IDLE);
        }

        ownerFSM.Play_FootStepSound();
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.RUN;
        ownerFSM.animator.SetBool("Run", false);
    }
}
