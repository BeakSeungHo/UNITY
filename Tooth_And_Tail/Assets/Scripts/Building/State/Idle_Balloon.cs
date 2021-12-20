using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    
    BuildingState Idle_Balloon()
    {
        return Idle_Defender();
    }
}
