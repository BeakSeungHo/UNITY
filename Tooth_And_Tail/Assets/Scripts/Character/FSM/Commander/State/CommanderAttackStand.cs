﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderAttackStand : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM;

    public CommanderAttackStand(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.ATTACK_STAND;
        ownerFSM.animator.SetBool("Run", false);
        ownerFSM.animator.SetBool("Attack", true);

        ownerFSM.Play_CommanderSound(ComSoundType.Attack);
    }

    public override void Run()
    {

        Vector3 move;

        bool isMove = ownerFSM.InputMove(out move);

        ownerFSM.commander.Move(move);

        if (ownerFSM.isAttack)
        {
            if (isMove)
                ownerFSM.ChangeFSM(CommanderFSM.STATE.ATTACK_RUN);

            if (ownerFSM.isCommandAll)
                ownerFSM.commander.Command_Attack_All();
            else
                ownerFSM.commander.Command_Atttack();
        }
        else
            ownerFSM.ChangeFSM(isMove ? CommanderFSM.STATE.RUN : CommanderFSM.STATE.IDLE);
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.ATTACK_STAND;
        ownerFSM.animator.SetBool("Attack", false);

    }
}
