using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    // 제분소나 두더지 땅굴의 경우 위에 돌아가는 부분이나 깃발 등이 따로 분리되어 있어서 추가하였다.
    public Animator topper;

    float curTime;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        if (data.CommonType == CommonType.Gristmill)
        {
            topper = gameObject.transform.Find("Topper").GetComponent<Animator>();
        }
    }

    public override void EnterState()
    {
        animator.SetBool("Idle", true);
    }

    public override void ExitState()
    {
        
    }

    public override BuildingState OperateState()
    {
        if (buildingBase.IsNeutral == true && data.CommonType == CommonType.Gristmill)
        {
            animator.SetTrigger("Idle_Neutral");
        }

        BuildingState retVal = BuildingState.End;

        switch (data.CommonType)
        {
            case CommonType.Gristmill:
                retVal = Idle_Gristmill();
                break;

            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
                retVal = Idle_Warrens();
                break;

            case CommonType.Turret:
            case CommonType.Cannon:
            case CommonType.Mine:
            case CommonType.Cabin:
                retVal = Idle_Defender();
                break;
            case CommonType.CampFire:
                retVal = Idle_Campfire();
                break;
            case CommonType.Wire:
                retVal = Idle_Wire();
                break;

            case CommonType.Balloon:
                retVal = Idle_Balloon();
                break;
                
        }
        return retVal;
    }
}