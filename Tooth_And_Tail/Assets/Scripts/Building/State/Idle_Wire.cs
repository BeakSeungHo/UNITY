using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    BuildingState Idle_Wire()
    {
        Camp camp = gameObject.GetComponent<CommonBase>().MyCamp;

        if (SquadController.Instance.Squads.ContainsKey(GameManager.Instance.CommanderList[1]))
        {
            List<Squad> squads = SquadController.Instance.Squads[GameManager.Instance.CommanderList[1]];

            for (int i = 0; i < squads.Count; i++)
            {
                foreach (CommonUnit unit in squads[i].UnitList)
                {
                    Vector2Int dist = TilemapSystem.Instance.RangeInObject(gameObject.transform.position, unit.transform.position, data.Range);
                    if (dist == TilemapSystem.Invalid_Range)
                        continue;

                    unit.Hit(data.Damage * Time.fixedDeltaTime, buildingBase);
                    buildingBase.Hit(data.Damage * Time.fixedDeltaTime, buildingBase);
                }
            }
        }
        return BuildingState.End;
    }
}