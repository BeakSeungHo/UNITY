using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Attack : IBuildingState
{
    
    void Attack_Mine()
    {
        animator.Play("Bomb");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bomb") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            //List<List<Squad>> squadList;
            Camp camp = gameObject.GetComponent<CommonBase>().MyCamp;

            if (SquadController.Instance.Squads.ContainsKey(GameManager.Instance.CommanderList[1]))
            {
                List<Squad> squads = SquadController.Instance.Squads[GameManager.Instance.CommanderList[1]];

                CommonUnit targetUnit = target.GetComponent<CommonUnit>();

                target.GetComponent<Character>().Hit(data.Damage, buildingBase);

                for (int i = 0; i < squads.Count; i++)
                {
                    foreach (CommonUnit unit in squads[i].UnitList)
                    {
                        Vector2Int dist = TilemapSystem.Instance.RangeInObject(gameObject.transform.position, unit.transform.position, 2);
                        if (dist == TilemapSystem.Invalid_Range || targetUnit == unit)
                            continue;

                        unit.Hit(data.Damage * 0.2f, buildingBase);
                    }
                }

                buildingBase.Play_Building_Sound(BuildSoundType.Attack);

                //  이펙트 추가 부분
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.EXPLOSION);

                buildingBase.DestroyBuilding();
            }
        }
    }
}
