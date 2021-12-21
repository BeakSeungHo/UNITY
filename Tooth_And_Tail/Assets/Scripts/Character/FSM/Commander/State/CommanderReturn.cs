using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderReturn : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM = null;

    private bool digKeyUp = false;

    public CommanderReturn(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.RETURN;
        digKeyUp = false;
        ownerFSM.CanBeTarget = false;

    }

    public override void Run()
    {
        if (BuildingManager.Instance.Buildings.ContainsKey(ownerFSM.commander.Base.MyCamp))
        {
            var buildings = BuildingManager.Instance.Buildings[ownerFSM.commander.Base.MyCamp];
            if (buildings.ContainsKey(CommonType.Gristmill))
            {
                var gristmills = buildings[CommonType.Gristmill];
                if (null == ownerFSM.commander.ReturnGristmillNode ||
                    !gristmills.Contains(ownerFSM.commander.ReturnGristmillNode.Value))
                {
                    ownerFSM.commander.ReturnGristmillNode = gristmills.First;
                    ownerFSM.commander.Set_RespawnPos(ownerFSM.commander.ReturnGristmillNode.Value.transform.position);
                }
            }
        }

        if (ownerFSM.InputReturn)
        {
            ownerFSM.ChangeFSM(CommanderFSM.STATE.IDLE);
            return;
        }

        if (!digKeyUp && !ownerFSM.InputDigging)
            digKeyUp = true;

        if (digKeyUp && ownerFSM.InputDigging)
            ownerFSM.ChangeFSM(CommanderFSM.STATE.IDLE);
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.RETURN;
        ownerFSM.CanBeTarget = true;
        ownerFSM.commander.ReturnGristmillNode = null;

        ownerFSM.Play_CommanderSound(Sound_Channel.Effect, ComSoundType.Return, 1);
    }
}