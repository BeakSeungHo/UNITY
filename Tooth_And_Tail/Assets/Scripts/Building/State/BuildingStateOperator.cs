using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState { Idle, Attack, Construct, Production, Death, End }

public enum BuildingOperators { Gristmill, Defender, Neutral }

public abstract class IBuildingState
{
    public static readonly int CHANGE_STATE = 1;
    public static readonly int NO_CHANGE = 0;

    protected BuildingBase buildingBase;

    public GameObject gameObject;

    public BuildingStateOperator stateOperator;

    public Animator animator;

    public CommonData data;

    protected GameObject target = null;

    public GameObject Target
    {
        set { target = value; }
    }

    public virtual void Initialize(GameObject obj)
    {
        gameObject = obj;
        animator = gameObject.transform.Find("Body").GetComponent<Animator>();
        buildingBase = gameObject.GetComponent<BuildingBase>();
        data = buildingBase.Base.Data;
        stateOperator = gameObject.GetComponent<BuildingStateOperator>();
    }

    public void SetData()
    {
        data = gameObject.GetComponent<CommonBase>().Data;
    }

    // 해당 빌딩이 공격할 대상을 찾는다.
    public bool SearchTarget()
    {
        Vector3 Pos = buildingBase.transform.position;
        int range = data.Range;

        Character character = null;
        bool found = false;
        if (buildingBase.Base.Type == CommonType.Wire || buildingBase.Base.Type == CommonType.Mine)
            found = InGameManager.Instance.Find_TargetInRange_ForMineWire(Pos, range, buildingBase.Base.MyCamp, buildingBase.Base.AttackType, out character) >= 0;
        else
            found = InGameManager.Instance.Find_TargetInRange_ExceptCamp(Pos, range, buildingBase.Base.MyCamp, buildingBase.Base.AttackType, out character) >= 0;

        if (character != null)
            target = character.gameObject;

        if (found == false)
            target = null;
        return found;
    }
    abstract public void EnterState();

    // 매 프레임 호출되는 함수
    abstract public BuildingState OperateState();

    abstract public void ExitState();
}


public partial class BuildingStateOperator : MonoBehaviour
{
    [SerializeField] protected BuildingState curBuildingState;

    protected IBuildingState curState = null;

    protected IBuildingState[] states;

    BuildingBase buildingBase = null;

    CommonBase commonBase = null;

    Animator animator;

    float percentHP;
    public BuildingState CurBuildingState
    {
        get
        {
            return curBuildingState;
        }
    }

    public void PlayCurTime()
    {
        ((BuildingState_Construct)(states[2])).PlayCurTime();
    }

    public float GetCurProductTime()
    {
        return ((State_Production)(states[3])).curProductTime;
    }

    public void MakeCampFire()
    {
        ((State_Idle)(states[0])).MakeCampfire();
    }

    public void Initialize()
    {
        if (states != null)
        {
            states[0].animator.Rebind();
            ChangeState(BuildingState.Construct);
            return;
        }
        // 상태들을 초기화 하여 각 Enum값에 맞게 배열에 넣는다.
        states = new IBuildingState[(int)BuildingState.End];
        states[0] = new State_Idle();
        states[1] = new State_Attack();
        states[2] = new BuildingState_Construct();
        states[3] = new State_Production();
        states[4] = new BuildingState_Construct();
        for (int i = 0; i < states.Length; i++)
        {
            states[i].Initialize(gameObject);
        }
        ChangeState(curBuildingState);

        commonBase = GetComponent<CommonBase>();
        buildingBase = GetComponent<BuildingBase>();
        animator = transform.Find("Body").GetComponent<Animator>();
    }

    public void SetStateData()
    {
        for (int i = 0; i < states.Length; i++)
        {
            states[i].data = commonBase.Data;
        }
    }

    // 중립 건물 점령
    public void Occupy(Camp camp)
    {
        buildingBase.IsNeutral = false;
        buildingBase.Base.MyCamp = camp;
        // BuildingManager.Instance.AddBuilding(camp, buildingBase);
    }

    // 해당 빌딩이 생산할 품목을 세팅한다.
    public void SetBuilding(CommonType type, Pool_ObjType poolType)
    {
        ((State_Production)states[3]).ProductionType = type;
        ((State_Production)states[3]).PoolType = poolType;
    }

    public void SetTarget(GameObject target)
    {
        ((State_Attack)states[1]).Target = target;
    }

    public void ShowTopper(bool show)
    {

        ((State_Idle)(states[0])).ShowTopper(show);

    }

    public void ChangeState(BuildingState state)
    {
        if (curState != null && curBuildingState != state)
            curState.ExitState();

        curBuildingState = state;

        curState = states[(int)state];

        curState.EnterState();
    }

    void Update()
    {
        if (buildingBase == null)
        {
            buildingBase = GetComponent<BuildingBase>();
        }
        percentHP = (buildingBase.HP / buildingBase.Base.MaxHp) * 100f;
        if (states == null)
        {
            Initialize();
        }
        if (curState == null)
        {
            ChangeState(BuildingState.Idle);
        }

        if (buildingBase.HP <= 0)
        {
            if (buildingBase.Base.MyCamp != Camp.End || buildingBase.Base.Type == CommonType.Cabin)
                buildingBase.DestroyBuilding();

            if (buildingBase.Base.MyCamp != GameManager.Instance.CommanderList[0])
            {
                switch (buildingBase.Base.Type)
                {
                    case CommonType.Wire:
                    case CommonType.Mine:
                    case CommonType.Turret:
                    case CommonType.Balloon:
                    case CommonType.Cannon:
                        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 4, 1);
                        break;
                }
                GameManager.Instance.MissionDestroyCount++;
            }
        }

        // 함수의 반환값이 End가 아니라면 리턴받은 상태로 상태를 변환시키는 구조.
        BuildingState retVal = curState.OperateState();

        if (retVal != BuildingState.End)
        {
            ChangeState(retVal);
        }

    }
    public Animator GetTopper()
    {
        return ((State_Idle)(states[0])).topper;
    }

    public void DestroyCampfire()
    {
        ((State_Idle)states[0]).DestroyCampfire();
    }

    public bool NowOccupyingCampfire()
    {
        return ((State_Idle)states[0]).NowOccupying();
    }
}