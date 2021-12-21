using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    BuildingState Idle_Defender()
    {
        SearchTarget();
        
        if (target != null)
        {
            stateOperator.SetTarget(target);
            return BuildingState.Attack;
        }
        return BuildingState.End;
    }
}
