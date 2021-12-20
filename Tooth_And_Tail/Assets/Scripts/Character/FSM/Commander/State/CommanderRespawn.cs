using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderRespawn : FSM<CommanderFSM>
{
    private CommanderFSM ownerFSM = null;
    
    public CommanderRespawn(CommanderFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = CommanderFSM.STATE.RESPAWN;
        ownerFSM.commander.RespawnTimeCount = 0f;
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

                    if (null != ownerFSM.commander.ReturnGristmillNode &&
                        null != ownerFSM.commander.ReturnGristmillNode.Value)
                        ownerFSM.commander.Set_RespawnPos(ownerFSM.commander.ReturnGristmillNode.Value.transform.position);
                }
            }
        }
    
        ownerFSM.commander.RespawnTimeCount += Time.deltaTime;

        if (ownerFSM.commander.RespawnTimeCount >= ownerFSM.commander.RespawnTime)
        {
            ownerFSM.commander.Respawn();
            ownerFSM.ChangeFSM(CommanderFSM.STATE.IDLE);
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = CommanderFSM.STATE.RESPAWN;
        ownerFSM.commander.ReturnGristmillNode = null;
    }
}
