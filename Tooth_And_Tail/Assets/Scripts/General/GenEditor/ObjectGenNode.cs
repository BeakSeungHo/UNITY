using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GenNode", menuName = "Data/GenNode")]
public class ObjectGenNode : ScriptableObject
{
    public GenNodeType nodeType;
    public Camp GenCamp;
    public List<Vector3> GenPosition;
    public CommonType Type;
    public CommonType ProductType;
    public int GenCount;
    public int Wave;
    [HideInInspector] public int PositionIndex = 0;

    [Header("젠과 동시에 발생시킬 이벤트")]
    public UnityEvent GenAction;

    public virtual IEnumerator GenStart()
    {
        WaitForSeconds wait = new WaitForSeconds(10f);

        while (true)
        {
            yield return wait;
            Gen();
        }
    }

    public void GenAndAssult()
    {
        int squadNum = SquadController.Instance.TypeToSquadNumber[GenCamp][Type];
        SquadController.Instance.Command_Move(GenCamp, squadNum, CampaignManager.Instance.startPosition.position);
    }

    public virtual void Gen()
    {
        PositionIndex = 0;
        for (int i = 0; i < GenCount; i++)
        {
            Pool_ObjType poolType;
            if (Type >= CommonType.Squirrel && Type <= CommonType.Fox)
                poolType = Pool_ObjType.Unit_Normal;
            else if (Type >= CommonType.Wire && Type <= CommonType.Cannon)
                poolType = Pool_ObjType.Building_Defender;
            else
                poolType = Pool_ObjType.Warrens;

            GameObject pullObj = PoolManager.Instance.PullObject(poolType);
            if (poolType == Pool_ObjType.Unit_Normal)
            {
                CommonUnit unit = pullObj.GetComponent<CommonUnit>();

                if (null == unit)
                    return;

                unit.Ready(GenCamp, Type, GenPosition[PositionIndex]);
                SquadController.Instance.Add_Unit(GenCamp, pullObj);
            }
            else
            {
                Vector3 position;
                if (GameManager.Instance.CurGameMode == GameMode.Campaign)
                    position = TilemapSystem.Instance.GetTile(GenPosition[PositionIndex]).worldPosition;
                else
                    position = GenPosition[PositionIndex];
                BuildingBase buildingBase = pullObj.GetComponent<BuildingBase>();
                buildingBase.transform.position = position;
                buildingBase.Base.Type = Type;
                buildingBase.Base.MyCamp = GenCamp;
                buildingBase.Product_Type = CommonType.End;
                if (poolType == Pool_ObjType.Warrens)
                    buildingBase.Product_Type = ProductType;
                buildingBase.Make_Building();
            }
            if (PositionIndex != GenPosition.Count - 1)
                PositionIndex++;
        }


        // 해당 노드에 이벤트가 비어있지 않다면 해당 이벤트를 실행 시킨다.
        if (GenAction.GetPersistentEventCount() != 0)
        {
            GenAction.Invoke();
        }
    }
    public void Speech_Event()
    {
        InGameManager.Instance.Commanders[0].SpeechEvent.EventStart(Wave);
    }
}
