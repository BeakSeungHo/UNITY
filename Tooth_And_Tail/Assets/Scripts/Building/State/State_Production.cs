using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Production : IBuildingState
{
    CommonType productionType;

    Pool_ObjType poolType;

    public float curProductTime = 0f;

    float productTime = 5f;

    public bool IsComplete = false;

    public CommonType ProductionType
    {
        set
        {
            productionType = value;
        }
    }

    public Pool_ObjType PoolType
    {
        set
        {
            poolType = value;
        }
    }

    public override void EnterState()
    {
        productTime = SceneStarter.Instance.GetData(productionType).GenTime;
    }

    public override void ExitState()
    {
        
    }

    public override BuildingState OperateState()
    {
        curProductTime += Time.deltaTime;

        // 생산 중에 CurUnit이 MaxUnit보다 커지면(건물을 판매해서 MaxUnit이 조정된 경우 등) 생산을 중단한다.
        int maxUnit = BuildingManager.Instance.maxUnits[buildingBase.Base.MyCamp][productionType];
        int curUnit = BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][productionType];
        if (maxUnit < curUnit)
        {
            BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][productionType]--;
            curProductTime = 0f;
            return BuildingState.Idle;
        }
        if (curProductTime >= productTime)
        {
            CommonBase commonBase = buildingBase.Base;
            GameObject unit = PoolManager.Instance.PullObject(poolType);

            unit.GetComponent<CommonUnit>().Ready(commonBase.MyCamp, productionType, gameObject.transform.position);
            curProductTime = 0f;

            SquadController.Instance.Add_Unit(commonBase.MyCamp, unit);

            switch(productionType)
            {
                case CommonType.Owl:
                    GameManager.Instance.MissionOwlCount++;
                    break;
                case CommonType.Wolf:
                    GameManager.Instance.MissionWolfCount++;
                    break;
                case CommonType.Chameleon:
                    GameManager.Instance.MissionChameleonCount++;
                    break;
            }

            if (false == BuildingManager.Instance.ProductCompleteProcess(commonBase.MyCamp, productionType, buildingBase))
            {
                return BuildingState.Idle;
            }
            return BuildingState.End;
        }
        return BuildingState.End;
    }
}
