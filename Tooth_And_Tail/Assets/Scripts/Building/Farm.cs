using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    public enum FarmState { Idle, Cultivation, Production, Death }

    [SerializeField] List<GameObject> wheats = new List<GameObject>();

    [SerializeField] Pig pig = null;

    GameObject wheatBundle;

    Animator animator;

    CommonBase commonBase = null;

    CommonBase gristmillBase;

    BuildingBase gristmillBuildingBase;
    public BuildingBase buildingBase;

    public HPCanvas hpCanvas = null;
    public WarningCanvas warningCanvas = null;

    float hpPerFrame;

    float constructTime = 5;
    float curConstructTime = 0f;

    float productionTime;
    float curProductionTime = 0f;

    public int food = 300;
    int wheatUsedUp = 0;
    int curWheatIndex = 0;

    [SerializeField] FarmState state;

    public bool exhaust = false;
    public bool queued = false;

    public BuildingBase GristmillBuildingBase
    {
        get { return gristmillBuildingBase; }
    }

    public void Queue()
    {
        queued = true;
        animator.SetBool("Cultivation", true);
        animator.SetBool("Idle", false);
    }

    public void CancelQueue()
    {
        queued = false;
        animator.SetBool("Cultivation", false);
        animator.SetBool("Idle", true);
        BuildingManager.Instance.EraseQueueFarm(buildingBase.Base.MyCamp);
        buildingBase.Base.MyCamp = Camp.End;
    }

    public bool IsActiveWheats()
    {
        return wheatBundle.activeInHierarchy;
    }

    public List<GameObject> GetWheats()
    {
        return wheats;
    }

    public CommonBase GetGristmillBase()
    {
        return gristmillBase;
    }

    public FarmState GetState()
    {
        return state;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.Find("Body").GetComponent<Animator>();
        state = FarmState.Idle;
        wheatBundle = transform.Find("WheatsBundle").gameObject;

        // 식량을 이 변수만큼 소모할 때 마다 밀이 사라짐
        wheatUsedUp = food / wheats.Count;

        // 밭에서 재배하는 밀의 인덱스를 섞어서 밀이 리스트에서
        // 순차적으로 사라져도 랜덤하게 사라지는 것 처럼 보이게 함.
        for (int i = 0; i < 10; i++)
        {
            int index1 = Random.Range(0, 8);
            int index2 = Random.Range(0, 8);
            GameObject temp = wheats[index1];
            wheats[index1] = wheats[index2];
            wheats[index2] = temp;
        }

        commonBase = GetComponent<CommonBase>();
        buildingBase = GetComponent<BuildingBase>();

        buildingBase.SetTileOccupy();

        gristmillBase = transform.parent.parent.GetComponent<CommonBase>();
        gristmillBuildingBase = gristmillBase.GetComponent<BuildingBase>();
        constructTime = commonBase.Data.GenTime;
        productionTime = commonBase.Data.AttackSpeed;
    }

    public void Cultivation(Camp camp)
    {
        if (GameManager.Instance.GetFood(camp) < commonBase.Data.Cost)
        {
            if (BuildingManager.Instance.QueueFarm(camp, this))
            {
                Queue();
                commonBase.MyCamp = camp;
                buildingBase.HP = 0.01f;
            }
            else
                Debug.Log("이미 큐가 꽉 찼습니다");
            return;
        }
        commonBase.MyCamp = camp;

        GameManager.Instance.ChangeFoodCamp(camp, -commonBase.Data.Cost);

        buildingBase.Play_BuildingGeneralSound(ComSoundType.Build, 0, 0.3f);

        if (commonBase.MyCamp == GameManager.Instance.CommanderList[0])
        {
            //숫자
            EffectManager.Instance.FontEffectEnable(InGameManager.Instance.Commanders[commonBase.MyCamp].gameObject, commonBase.Data.Cost, FontEffect.FONTTYPE.MINUSFOOD);
        }

        hpPerFrame = commonBase.Data.MaxHp / commonBase.Data.GenTime;

        curConstructTime = 0f;

        state = FarmState.Cultivation;

        buildingBase.HP = 0.01f;

        animator.SetBool("Cultivation", true);
        animator.SetBool("Idle", false);

        hpCanvas.Ready();
        buildingBase.SetMiniSpriteColor();

        BuildingManager.Instance.AddBuilding(camp, buildingBase);
    }

    public void Production(bool isDefaultFarm = false, Camp camp = Camp.End)
    {
        state = FarmState.Production;
        wheatBundle.SetActive(true);
        animator.SetBool("Idle", false);
        animator.SetBool("Cultivation", false);
        animator.SetBool("Production", true);

        if (isDefaultFarm)
        {
            commonBase.MyCamp = camp;
            buildingBase.HP = commonBase.Data.MaxHp;
            BuildingManager.Instance.AddBuilding(camp, buildingBase);
            hpCanvas.Ready();
            buildingBase.SetMiniSpriteColor();
        }

        for (int i = 0; i < 9; i++)
        {
            wheats[i].SetActive(true);
        }

        pig.gameObject.SetActive(true);
        warningCanvas.gameObject.SetActive(false);

        pig.SetPig(commonBase.MyCamp, !isDefaultFarm);
    }

    public void Destroy()
    {
        if (queued)
            CancelQueue();
        else
        {
            // 상태가 변경되기전에 생산상태일때 피그가 죽으면 경고 표시
            if (state == FarmState.Production && food > 0)
                warningCanvas.gameObject.SetActive(true);

            state = FarmState.Idle;
            animator.SetBool("Idle", true);
            animator.SetBool("Cultivation", false);
            animator.SetBool("Production", false);

            if (!exhaust)
                pig.Die();

            buildingBase.HP = 0;

            curConstructTime = 0f;

            BuildingManager.Instance.DeleteBuilding(commonBase.MyCamp, buildingBase);
            commonBase.MyCamp = Camp.End;
        }
    }

    public bool BuildOrSell(Camp camp)
    {
        CommonBase gristmillBase = GetGristmillBase();
        // 농장을 포함하는 제분소가 건설중이지 않을 경우
        if (gristmillBase.MyCamp == camp && gristmillBase.GetComponent<BuildingStateOperator>().CurBuildingState != BuildingState.Construct)
        {
            if (GetState() == Farm.FarmState.Idle)
            {
                Cultivation(camp);
            }
            else if (GetState() == Farm.FarmState.Cultivation)
            {
                commonBase.GetComponent<BuildingBase>().Sell();
            }
            else
            {
                Debug.Log("건설이 이미 완료되었습니다");
                return false;
            }
        }
        return true;
    }

    void CheckState()
    {
        if (!gristmillBuildingBase.GetAnimator().GetBool("Idle"))
        {
            curConstructTime = 0;
            buildingBase.DestroyBuilding();
        }
    }

    // 간단하게 Update안에서 상태별로 행동을 구현함
    void Update()
    {
        bool endFlag = false;
        if (state != FarmState.Idle && buildingBase.HP <= 0)
        {
            buildingBase.DestroyBuilding();
            endFlag = true;
        }

        if (exhaust || endFlag)
        {
            return;
        }

        switch (state)
        {
            case FarmState.Cultivation:
                CheckState();
                curConstructTime += Time.deltaTime;
                buildingBase.HP += Time.deltaTime * hpPerFrame;
                BuildingManager.Instance.AddFoodProductionAmount(commonBase.MyCamp);
                if (curConstructTime >= constructTime)
                {
                    if (buildingBase.HP > commonBase.Data.MaxHp)
                        buildingBase.HP = commonBase.Data.MaxHp;
                    Production();
                }
                break;

            case FarmState.Production:
                CheckState();
                curProductionTime += Time.deltaTime;
                BuildingManager.Instance.AddFoodProductionAmount(commonBase.MyCamp);
                if (curProductionTime >= productionTime && curWheatIndex < wheats.Count)
                {
                    // 식량 생산 코드
                    food--;
                    GameManager.Instance.ChangeFoodCamp(commonBase.MyCamp, 1);

                    // 통계 식량 데이터 갱신
                    SceneStarter.Instance.statisticElements.UpdateFood(commonBase.MyCamp);

                    if (commonBase.MyCamp == GameManager.Instance.CommanderList[0])
                    {
                        //숫자
                        EffectManager.Instance.FontEffectEnable(pig.gameObject, 1, FontEffect.FONTTYPE.PLUSFOOD);
                    }
                    if (food < 297 && food % wheatUsedUp == 0 && food > 0)
                    {
                        //Debug.Log(food);
                        wheats[curWheatIndex].SetActive(false);
                        curWheatIndex++;
                    }
                    if (food == 0)
                    {
                        wheats[wheats.Count - 1].SetActive(false);
                        curWheatIndex = wheats.Count;

                        Destroy();

                        animator.SetBool("Exhaust", true);
                        exhaust = true;
                    }
                    curProductionTime = 0f;
                }
                break;
        }
    }
    private void LateUpdate()
    {
        UpdateOutline();
    }
    void UpdateOutline()
    {
        if (commonBase.MyCamp != GameManager.Instance.CommanderList[0])
        {
            if (InGameManager.Instance.Commanders[GameManager.Instance.CommanderList[0]].AttackTarget == buildingBase)
            {
                if (pig.OutLine != null)
                    pig.OutLine.outlineSize = 1;
            }
            else
            {
                if (pig.OutLine != null)
                    pig.OutLine.outlineSize = 0;
            }
        }
    }

}
