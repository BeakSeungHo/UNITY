using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    BuildingState Idle_Warrens()
    {
        bool retVal = BuildingManager.Instance.ProductCompleteProcess(buildingBase.Base.MyCamp, buildingBase.Product_Type, buildingBase);
        if (retVal)
            return BuildingState.Production;
        return BuildingState.End;
    }
}