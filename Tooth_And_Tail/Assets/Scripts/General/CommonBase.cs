using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Camp { Error = -1, Bellafide, Hopper, Quartermaster, Archimedes, End }
public class CampComparer : IEqualityComparer<Camp>
{
    public bool Equals(Camp x, Camp y)
    {
        return x == y;
    }

    public int GetHashCode(Camp obj)
    {
        return (int)obj;
    }
}
public class CommonBase : MonoBehaviour
{
    public Camp MyCamp;

    public CommonType Type;

    public Pool_ObjType PoolType;

    public int Reinforcement = 0;

    [SerializeField]
    private BuffDebuff buffDebuff = null;

    public CommonElements elements
    {
        get
        {
            return SceneStarter.Instance.commonElements;
        }
    }

    public ReinforceElements reinforceElements
    {
        get
        {
            return SceneStarter.Instance.reinforceElements;
        }
    }

    public CommonData Data
    {
        get
        {
            return elements.CommonDataList[(int)Type];
        }
    }

    public CommonData GetData(CommonType type)
    {
        return elements.CommonDataList[(int)type];
    }

    public Pool_ObjType GetPoolType(CommonType type)
    {
        return elements.CommonDataList[(int)type].PoolType;
    }


    public PlaceType PlaceType { get { return elements.CommonDataList[(int)Type].PlaceType; } }
    public AttackType AttackType { get { return elements.CommonDataList[(int)Type].AttackType; } }

    public int UnitPerBuilding { get { return elements.CommonDataList[(int)Type].UnitPerBuliding + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).UnitPerBuliding; } }
    public int Sight { get { return elements.CommonDataList[(int)Type].Sight + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).Sight; } }
    public int Range { get { return elements.CommonDataList[(int)Type].Range + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).Range; } }
    public int Cost { get { return elements.CommonDataList[(int)Type].Cost + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).Cost; } }

    public float Damage { get { return elements.CommonDataList[(int)Type].Damage + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).Damage; } }
    public float MaxHp { get { return elements.CommonDataList[(int)Type].MaxHp + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).MaxHp; } }
    public float AttackSpeed { get { return (elements.CommonDataList[(int)Type].AttackSpeed + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).AttackSpeed) * (buffDebuff.Stim ? 2f : 1f); } }
    public float ProjectileSpeed { get { return elements.CommonDataList[(int)Type].ProjectileSpeed + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).ProjectileSpeed; } }
    public float MoveSpeed { get { return (elements.CommonDataList[(int)Type].MoveSpeed + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).MoveSpeed) * (buffDebuff.Stim ? 2f : 1f); } }
    public float GenTime { get { return elements.CommonDataList[(int)Type].GenTime + reinforceElements.GetReinforceAccCurData(Type, Reinforcement).GenTime; } }

    public float BuildTime { get { return elements.CommonDataList[(int)Type].GenTime * (GameManager.Instance.CurGameMode == GameMode.Tutorial ? 0.3f : 1); } }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public virtual void ReturnPool()
    {
        PoolManager.Instance.PushObject(gameObject, PoolType);
    }
}
