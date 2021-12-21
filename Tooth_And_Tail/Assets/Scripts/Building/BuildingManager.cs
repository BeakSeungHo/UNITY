using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    // 게임에 참가한 진영의 수
    public int CommanderCount;

    // 유닛 생산관리에 사용될 변수들.
    public Dictionary<Camp, Dictionary<CommonType, int>> maxUnits;
    public Dictionary<Camp, Dictionary<CommonType, int>> curUnits;

    Dictionary<Camp, int> foodProductionAmount;

    //Dictionary<Camp, List<GameObject>> buildings;
    public Dictionary<Camp, Dictionary<CommonType, LinkedList<BuildingBase>>> Buildings;

    public Dictionary<Camp, Farm> farmQueue;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        maxUnits = new Dictionary<Camp, Dictionary<CommonType, int>>();
        curUnits = new Dictionary<Camp, Dictionary<CommonType, int>>();
        Buildings = new Dictionary<Camp, Dictionary<CommonType, LinkedList<BuildingBase>>>();

        foodProductionAmount = new Dictionary<Camp, int>();

        farmQueue = new Dictionary<Camp, Farm>();

        foreach (Camp camp in GameManager.Instance.CommanderList)
        {
            AddCamp(camp);
            farmQueue.Add(camp, null);
        }
        AddCamp(Camp.End);
    }

    public Camp GetBuildinglessCamp()
    {
        foreach (var keyValue in Buildings)
        {
            if (keyValue.Value.Count == 0)
                return keyValue.Key;
        }
        return Camp.Error;
    }

    void Update()
    {
        foreach (Camp camp in GameManager.Instance.CommanderList)
        {
            if (farmQueue[camp] != null)
            {
                int food = GameManager.Instance.GetFood(camp);
                if (food >= SceneStarter.Instance.GetData(CommonType.Farm).Cost)
                {
                    farmQueue[camp].Cultivation(camp);
                    farmQueue[camp].queued = false;
                    farmQueue[camp] = null;
                }
            }
        }
    }

    public bool IsOnProductionFood(Camp camp)
    {
        if (Buildings[camp][CommonType.Farm].Count > 0)
            return true;

        if (Buildings[camp].ContainsKey(CommonType.CampFire))
        {
            return Buildings[camp][CommonType.CampFire].Count > 0;
        }
        return false;
    }

    public void AddFoodProductionAmount(Camp camp)
    {
        if (foodProductionAmount[camp] != -1)
            foodProductionAmount[camp]++;
    }

    public void DestroyAllBuilding(Camp camp)
    {
        foreach (var buildingList in Buildings[camp])
        {
            foreach (var building in buildingList.Value)
            {
                building.HP = 0;
            }
        }
    }

    public bool IsGameOver(Camp camp)
    {
        int gristmillCount = 0;
        if (Buildings[camp].ContainsKey(CommonType.Gristmill))
            gristmillCount = Buildings[camp][CommonType.Gristmill].Count;
        int campFireCount = 0;
        if (Buildings[camp].ContainsKey(CommonType.CampFire))
            campFireCount = Buildings[camp][CommonType.CampFire].Count;
        bool retVal = gristmillCount + campFireCount == 0;

        if (retVal)
        {
            foreach (var buildingList in Buildings[camp])
            {
                foreach (var building in buildingList.Value)
                {
                    building.HP = 0;
                }
            }
        }

        return retVal;
    }

    public bool QueueFarm(Camp camp, Farm farm)
    {
        if (farmQueue[camp] != null)
            return false;

        farmQueue[camp] = farm;
        return true;
    }

    public void EraseQueueFarm(Camp camp)
    {
        if (farmQueue[camp] != null)
            farmQueue[camp] = null;
    }

    public void AddCamp(Camp camp)
    {
        maxUnits.Add(camp, new Dictionary<CommonType, int>());
        curUnits.Add(camp, new Dictionary<CommonType, int>());
        Buildings.Add(camp, new Dictionary<CommonType, LinkedList<BuildingBase>>());

        foodProductionAmount.Add(camp, 4);

        //현재는 테스트용. 추후에 선택한 6가지 유닛을 받아올 수 있으면 그 받아온 타입을 사용할 예정
        for (int i = (int)CommonType.Squirrel; i <= (int)CommonType.Fox; ++i)
        {
            maxUnits[camp].Add((CommonType)i, 0);
            curUnits[camp].Add((CommonType)i, 0);
        }

    }

    public void AddUnits(Camp camp, CommonType type)
    {
        curUnits[camp][type]++;
    }

    public void AddBuilding(Camp camp, BuildingBase building)
    {
        if (!Buildings[camp].ContainsKey(building.Base.Type))
        {
            Buildings[camp].Add(building.Base.Type, new LinkedList<BuildingBase>());
        }
        Buildings[camp][building.Base.Type].AddLast(building);
    }

    public void DeleteBuilding(Camp camp, BuildingBase building)
    {
        if (!Buildings[camp][building.Base.Type].Remove(building))
        {
            Debug.Log("빌딩 제거 실패");
        }
    }

    // 건물이 지어졌을 때 해당 건물이 생산할 유닛의 최대 갯수를 늘리고 추가로 생산 가능한 건물을 탐색하여 생산 명령을 내린다.
    public bool AddMaxUnits(Camp camp, BuildingBase buildingBase)
    {
        CommonType product_type = buildingBase.Product_Type;

        CommonType warrensType = buildingBase.Base.Data.CommonType;

        if (warrensType == CommonType.MoleeMerge)
        {
            int extraUnits = SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(product_type, SceneStarter.Instance.userElements.GetLevel(product_type)).UnitPerBuliding;
            maxUnits[camp][product_type] += (3 + extraUnits);
        }
        else
        {
            // 1티어는 건물당 3마리 2티어는 건물당 2마리 3티어는 건물당 1마리씩 증가
            int extraUnits = SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(product_type, SceneStarter.Instance.userElements.GetLevel(product_type)).UnitPerBuliding;
            maxUnits[camp][product_type] += ((int)CommonType.WarrenT3 - ((int)warrensType - 1)) + extraUnits;
        }

        bool retVal;
        retVal = CheckCanProduction(camp, product_type);
        return retVal;
    }

    public void DeathUnit(Camp camp, CommonType type)
    {
        curUnits[camp][type]--;
    }

    // 현재 타입의 오브젝트를 생산할 수 있는 건물을 찾아서 생산 명령을 내린다.
    public bool CheckCanProduction(Camp camp, CommonType type)
    {
        bool retVal = false;
        if (maxUnits[camp].Count != 0)
        {
            if (curUnits[camp][type] < maxUnits[camp][type])
            {
                foreach (var dic in Buildings[camp])
                {
                    if (dic.Key == CommonType.WarrenT1 || dic.Key == CommonType.WarrenT2 || dic.Key == CommonType.WarrenT3 || dic.Key == CommonType.MoleeMerge)
                    {
                        foreach (var building in dic.Value)
                        {
                            if (building.Product_Type != type)
                            {
                                continue;
                            }
                            int Cost = SceneStarter.Instance.GetData(building.Product_Type).Cost;
                            // 유닛 생산 가능한 상태고 식량이 충분할 경우
                            if (Cost <= GameManager.Instance.GetFood(building.Base.MyCamp) &&
                                building.GetCurState() == BuildingState.Idle && curUnits[camp][type] < maxUnits[camp][type])
                            {
                                building.ChangeState(BuildingState.Production);
                                GameManager.Instance.ChangeFoodCamp(building.Base.MyCamp, -Cost);
                                if (building.Base.MyCamp == GameManager.Instance.CommanderList[0])
                                {
                                    //숫자
                                    EffectManager.Instance.FontEffectEnable(building.gameObject, Cost, FontEffect.FONTTYPE.MINUSFOOD);
                                }
                                curUnits[camp][type]++;
                                retVal = true;
                            }
                        }
                    }
                }
            }
        }
        return retVal;
    }

    // 해당 건물의 생산이 완료되면 추가 생산 여부를 판별한다.
    public bool ProductCompleteProcess(Camp camp, CommonType type, BuildingBase buildingBase)
    {
        int Cost = SceneStarter.Instance.reinforceElements.CompleteReinforceCurData(type, SceneStarter.Instance.userElements.GetLevel(type)).Cost;
        if (Cost <= GameManager.Instance.GetFood(buildingBase.Base.MyCamp) &&
            curUnits[camp][buildingBase.Product_Type] < maxUnits[camp][buildingBase.Product_Type])
        {
            GameManager.Instance.ChangeFoodCamp(buildingBase.Base.MyCamp, -Cost);
            if (buildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
            {
                //숫자
                EffectManager.Instance.FontEffectEnable(buildingBase.gameObject, Cost, FontEffect.FONTTYPE.MINUSFOOD);
            }
            curUnits[camp][buildingBase.Product_Type]++;
            return true;
        }
        return false;
    }
}