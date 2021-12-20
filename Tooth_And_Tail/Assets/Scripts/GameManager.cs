using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameMode { None, Tutorial, Campaign, Multi, TimeAttack }

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public int CampCount = 2;
    public int UnitCount = 6;
    public Camp CommanderType = Camp.Hopper;

    public Dictionary<Camp, int> CampFoodDic = new Dictionary<Camp, int>();
    public Dictionary<Camp, List<CommonType>> UnitTypeDic = new Dictionary<Camp, List<CommonType>>();
    public Dictionary<Camp, Dictionary<CommonType, int>> CampUnitReinforcement = new Dictionary<Camp, Dictionary<CommonType, int>>();
    public List<Camp> CommanderList = new List<Camp>();

    LinkedList<CommonType> UnitTypeLinkedList = new LinkedList<CommonType>();
    List<CommonType> UnitTypeList = new List<CommonType>();

    public int MissionKillCount = 0;                    // 한판당 유닛 죽인 횟수
    public int MissionDestroyCount = 0;                 // 한판당 건물 부신 횟수
    public int MissionOwlCount = 0;                     // 한판당 올빼미 소환 횟수
    public int MissionWolfCount = 0;                    // 한판당 늑대 소환 횟수
    public int MissionChameleonCount = 0;               // 한판당 카멜레온 소환 횟수

    public GameMode CurGameMode = GameMode.None;        //게임 모드 설정

    public Camp WinnerCamp = Camp.End;                  // 승리 캠프
    public Camp LoserCamp = Camp.End;                   // 패배 캠프

    public int EndGameRewardGold = 0;                   // 게임보상 골드 
    public int EndGameRewardExp = 0;                    // 게임보상 경험치
    public List<ShopItemData> EndGameRewardList;        // 게임보상 리스트

    public int ReceivedMissionCount = 0;                // 수령한 미션 카운트
    public int CompleteMissionCount = 0;                // 완료한 미션 카운트

    public float ChargeStaminaTimeCount = 0;            // 스태미나 시간 계산용
    bool bIsChargingStamina = false;                    // 충전여부 체크
    
    void Awake()
    {
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (SceneStarter.Instance.userElements.ChargeStaminaTime != TimeSpan.Zero)
            ChargeStaminaTimeCount = (float)SceneStarter.Instance.userElements.ChargeStaminaTime.TotalSeconds;
        else
            ChargeStaminaTimeCount = Global.StaminaChargeTime; // 10분

        DateTime TargetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        TargetTime = TargetTime.AddDays(1);

        TimeSpan ts = TargetTime - DateTime.Now;

        Invoke("CheckMidNight", (float)ts.TotalSeconds);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        CalculationBoosterTime();
        if ((SceneStarter.Instance.userElements.UserData.UserCurStamina <
            SceneStarter.Instance.userElements.UserData.UserMaxStamina) &&
            !bIsChargingStamina)
            StartCoroutine(ChargeStamina());
    }

    // 배틀레디씬에서 커맨더 타입 설정 함수
    public void SetCommanderType(Camp MyCamp)
    {
        CommanderType = MyCamp;
    }

    // 배틀레디씬에서 유닛 추가
    public bool SetUnitType(int Index)
    {
        if (UnitTypeLinkedList.Contains((CommonType)Index))
            UnitTypeLinkedList.Remove((CommonType)Index);
        else if (UnitTypeLinkedList.Count >= 6)
            return false;
        else
            UnitTypeLinkedList.AddLast((CommonType)Index);

        return true;
    }

    // 선택한 유닛 초기화하는 함수
    public void ResetUnitList()
    {
        CommanderType = Camp.Bellafide;
        UnitTypeLinkedList.Clear();
        UnitTypeList.Clear();
    }

    // 게임시작시 유닛을 List에 넣고 정렬하는 함수
    public bool CompleteList()
    {
        if (UnitTypeLinkedList.Count < UnitCount)
            return false;

        UnitTypeDic.Clear();
        CommanderList.Clear();
        CampFoodDic.Clear();
        CampUnitReinforcement.Clear();
        UnitTypeDic.Add(CommanderType, new List<CommonType>());
        CampUnitReinforcement.Add(CommanderType, new Dictionary<CommonType, int>());
        CommanderList.Add(CommanderType);

        var LinkedEnumer = UnitTypeLinkedList.GetEnumerator();

        while (LinkedEnumer.MoveNext())
        {
            UnitTypeDic[CommanderType].Add(LinkedEnumer.Current);
            CampUnitReinforcement[CommanderType].Add(LinkedEnumer.Current, SceneStarter.Instance.userElements.UnitReinforceMent(LinkedEnumer.Current));
        }
        UnitTypeDic[CommanderType].Sort();

        CampFoodDic.Add(CommanderType, 0);

        SetAICampAndUnit();

        return true;
    }

    // 튜토리얼 셋팅
    public void SetTutorial()
    {
        CommanderList.Clear();
        CampFoodDic.Clear();
        UnitTypeDic.Clear();
        CampUnitReinforcement.Clear();

        CommanderList.Add(Camp.Bellafide);
        CampFoodDic.Add(Camp.Bellafide, 9999);
        UnitTypeDic.Add(Camp.Bellafide, new List<CommonType>());
        CampUnitReinforcement.Add(Camp.Bellafide, new Dictionary<CommonType, int>());

        CommanderList.Add(Camp.Archimedes);
        CampFoodDic.Add(Camp.Archimedes, 0);
        UnitTypeDic.Add(Camp.Archimedes, new List<CommonType>());
        CampUnitReinforcement.Add(Camp.Archimedes, new Dictionary<CommonType, int>());

        UnitTypeDic[Camp.Archimedes].Add(CommonType.Toad);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Skunk);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Pigeon);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Falcon);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Toad, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Skunk, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Pigeon, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Falcon, 0);

        UnitTypeDic[Camp.Bellafide].Add(CommonType.Squirrel);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Ferret);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Turret);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Balloon);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Falcon);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Boar);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Squirrel, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Ferret, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Turret, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Balloon, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Falcon, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Boar, 0);
    }

    // 캠페인 셋팅
    public void SetCampaign()
    {
        CommanderList.Clear();
        CampFoodDic.Clear();
        UnitTypeDic.Clear();
        CampUnitReinforcement.Clear();

        CommanderList.Add(Camp.Bellafide);
        CampFoodDic.Add(Camp.Bellafide, 600);
        UnitTypeDic.Add(Camp.Bellafide, new List<CommonType>());
        CampUnitReinforcement.Add(Camp.Bellafide, new Dictionary<CommonType, int>());

        CommanderList.Add(Camp.Archimedes);
        CampFoodDic.Add(Camp.Archimedes, 0);
        UnitTypeDic.Add(Camp.Archimedes, new List<CommonType>());
        CampUnitReinforcement.Add(Camp.Archimedes, new Dictionary<CommonType, int>());

        UnitTypeDic[Camp.Archimedes].Add(CommonType.Toad);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Badger);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Pigeon);
        UnitTypeDic[Camp.Archimedes].Add(CommonType.Falcon);

        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Toad, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Badger, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Pigeon, 0);
        CampUnitReinforcement[Camp.Archimedes].Add(CommonType.Falcon, 0);

        UnitTypeDic[Camp.Bellafide].Add(CommonType.Squirrel);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Ferret);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Turret);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Boar);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Fox);
        UnitTypeDic[Camp.Bellafide].Add(CommonType.Chameleon);

        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Squirrel, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Ferret, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Turret, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Boar, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Fox, 0);
        CampUnitReinforcement[Camp.Bellafide].Add(CommonType.Chameleon, 0);
    }

    // 플레이어의 인덱스값에 따른 유닛타입 가져오는 함수
    public CommonType PlayerUnitType(int UnitIndex)
    {
        if (UnitIndex >= UnitCount)
            return 0;

        return UnitTypeDic[CommanderList[0]][UnitIndex];
    }

    // 캠프의 인덱스값에 따른 유닛타입 가져오는 함수
    public CommonType GetUnitType(Camp camp, int UnitIndex)
    {
        if (UnitIndex >= UnitCount)
            return 0;

        return UnitTypeDic[camp][UnitIndex];
    }

    // 플레이어의 식량 가져오는 함수
    public int GetFoodPlayer()
    {
        return CampFoodDic[CommanderList[0]];
    }

    // 플레이어의 식량 증가함수
    public void ChangeFoodPlayer(int AddFoodNum)
    {
        //Debug.Log("플레이어의 식량 증가함수");
        CampFoodDic[CommanderList[0]] += AddFoodNum;
    }

    // 캠프의 식량 증가 함수
    public void ChangeFoodCamp(Camp campType, int AddFoodNum)
    {
        //Debug.Log("ChangeFoodCamp : " + campType);
        CampFoodDic[campType] += AddFoodNum;
    }

    // 캠프의 식량 가져오는 함수
    public int GetFood(Camp camp)
    {
        return CampFoodDic[camp];
    }

    //AI Camp와 Unit 랜덤으로 설정
    void SetAICampAndUnit()
    {
        //임시 AI

        Camp aiCamp = CommanderList[0] != Camp.Bellafide ? Camp.Bellafide : Camp.Archimedes;

        CampFoodDic.Add(aiCamp, 0);

        CommanderList.Add(aiCamp);

        UnitTypeDic.Add(aiCamp, new List<CommonType>());
        CampUnitReinforcement.Add(aiCamp, new Dictionary<CommonType, int>());

        //for (int i = 0; i < UnitCount; ++i)
        //{
        //    while (true)
        //    {
        //        CommonType AIUnitType = (CommonType)Random.Range(0, 20);

        //        if (!UnitTypeDic[Camp.Archimedes].Contains(AIUnitType))
        //        {
        //            UnitTypeDic[Camp.Archimedes].Add(AIUnitType);
        //            break;
        //        }
        //    }
        //}

        UnitTypeDic[aiCamp].Add(CommonType.Squirrel);
        UnitTypeDic[aiCamp].Add(CommonType.Toad);
        UnitTypeDic[aiCamp].Add(CommonType.Pigeon);
        UnitTypeDic[aiCamp].Add(CommonType.Ferret);
        UnitTypeDic[aiCamp].Add(CommonType.Skunk);
        UnitTypeDic[aiCamp].Add(CommonType.Boar);
        CampUnitReinforcement[aiCamp].Add(CommonType.Squirrel, 0);
        CampUnitReinforcement[aiCamp].Add(CommonType.Toad, 0);
        CampUnitReinforcement[aiCamp].Add(CommonType.Pigeon, 0);
        CampUnitReinforcement[aiCamp].Add(CommonType.Ferret, 0);
        CampUnitReinforcement[aiCamp].Add(CommonType.Skunk, 0);
        CampUnitReinforcement[aiCamp].Add(CommonType.Boar, 0);

        UnitTypeDic[aiCamp].Sort();
        /////

        while (CommanderList.Count < CampCount)
        {
            Camp AICamp = 0;
            CommonType AIUnitType = 0;
            while (true)
            {
                AICamp = (Camp)UnityEngine.Random.Range(0, 4);

                if (!CommanderList.Contains(AICamp))
                {
                    CommanderList.Add(AICamp);
                    CampFoodDic.Add(AICamp, 0);
                    break;
                }
            }

            UnitTypeDic.Add(AICamp, new List<CommonType>());

            for (int i = 0; i < UnitCount; ++i)
            {
                while (true)
                {
                    AIUnitType = (CommonType)UnityEngine.Random.Range(0, 20);

                    if (!UnitTypeDic[AICamp].Contains(AIUnitType))
                    {
                        UnitTypeDic[AICamp].Add(AIUnitType);
                        break;
                    }
                }
            }
            UnitTypeDic[AICamp].Sort();
        }
    }

    public void WinnerReward()
    {
        EndGameRewardGold = 2000;
        EndGameRewardExp = 20;

        if (SceneStarter.Instance.userElements.UserData.bIsBooster)
            SceneStarter.Instance.userElements.SetBoosterExpAndGold(EndGameRewardExp, EndGameRewardGold, Global.BoosterPercent);
        else
            SceneStarter.Instance.userElements.AddExpAndGold(EndGameRewardExp, EndGameRewardGold);

        if (CurGameMode >= GameMode.Campaign)
            WinnerBox();
    }

    public void LoserReward()
    {
        EndGameRewardGold = 1000;
        EndGameRewardExp = 10;

        if (SceneStarter.Instance.userElements.UserData.bIsBooster)
            SceneStarter.Instance.userElements.SetBoosterExpAndGold(EndGameRewardExp, EndGameRewardGold, Global.BoosterPercent);
        else
            SceneStarter.Instance.userElements.AddExpAndGold(EndGameRewardExp, EndGameRewardGold);

        if (CurGameMode >= GameMode.Campaign)
            LoserBox();
    }

    public List<ShopItemData> GetEndRewardList()
    {
        return EndGameRewardList;
    }

    public void EndGame(Camp loserCamp)
    {
        for (int i = 0; i < CommanderList.Count; ++i)
        {
            if (CommanderList[i] == loserCamp)
                LoserCamp = loserCamp;
            else
                WinnerCamp = CommanderList[i];
        }

        // 최종 통계 저장
        SceneStarter.Instance.statisticElements.GameEndStatistic(loserCamp);

        BattleUICtrl.Instance.SetEndUI();

        switch (CurGameMode)
        {
            case GameMode.Tutorial:
                SceneStarter.Instance.userElements.SetTutorialMissions();
                break;
        }

        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Daily, 3, MissionKillCount);
        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 7, MissionKillCount);
        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 6, MissionDestroyCount);

        if (MissionOwlCount >= 10)
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 9, MissionOwlCount);
        if (MissionWolfCount >= 5)
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 11, MissionOwlCount);
        if (MissionChameleonCount >= 9)
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 12, MissionOwlCount);

        MissionKillCount = 0;
        MissionDestroyCount = 0;

        if (WinnerCamp == CommanderList[0])
        {
            WinnerReward();
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Daily, 0, 1);
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 0, 1);
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 1, 1);

            switch (CurGameMode)
            {
                case GameMode.Tutorial:
                    SceneStarter.Instance.userElements.SetChapter(0, true);
                    break;
                case GameMode.Campaign:
                    if(!SceneStarter.Instance.userElements.GetCurChapter(1))
                    {
                        SceneStarter.Instance.userElements.SetChapter(1, true);
                        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 0, 1);
                    }
                    break;
            }
        }
        else
            LoserReward();

        ResetUnitList();

        // 인게임 BGM 끄기
        SoundManager.Instance.Stop_BGM();

        CurGameMode = GameMode.None;
    }

    public IEnumerator LoadScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Lobby");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(3f);
                asyncOperation.allowSceneActivation = true;
            }
        }
    }

    void CalculationBoosterTime()
    {
        if (SceneStarter.Instance.userElements.UserData.bIsBooster)
        {
            SceneStarter.Instance.userElements.RestBoosterTime -= (DateTime.Now - SceneStarter.Instance.userElements.FirstBoosterTime);
            SceneStarter.Instance.userElements.FirstBoosterTime = DateTime.Now;

            if (SceneStarter.Instance.userElements.RestBoosterTime == TimeSpan.Zero)
                SceneStarter.Instance.userElements.UserData.bIsBooster = false;
        }
    }

    IEnumerator ChargeStamina()
    {
        UserElements userElements = SceneStarter.Instance.userElements;

        while (userElements.UserData.UserCurStamina < userElements.UserData.UserMaxStamina)
        {
            bIsChargingStamina = true;

            ChargeStaminaTimeCount--;

            if (ChargeStaminaTimeCount == 0)
            {
                ChargeStaminaTimeCount = Global.StaminaChargeTime;
                userElements.UserData.UserCurStamina++;
                Debug.Log(userElements.UserData.UserCurStamina);
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        ChargeStaminaTimeCount = Global.StaminaChargeTime;
        bIsChargingStamina = false;
    }

    void CheckMidNight()
    {
        SceneStarter.Instance.userElements.CheckReset();
    }

    List<ShopItemData> WinnerBox()
    {
        EndGameRewardList.Clear();

        Dictionary<ItemType, ShopItemData> RandomItemDic = new Dictionary<ItemType, ShopItemData>();
        UserElements TempUserElements = SceneStarter.Instance.userElements;

        ItemType RandItemType = ItemType.None;


        for (int i = 0; i < 5; ++i)
        {
            float RandomNum = UnityEngine.Random.Range(0.0f, 100.0f);

            if (RandomNum >= 0 && RandomNum <= 3)
                RandItemType = ItemType.CheeseHigh;
            else if (RandomNum >= 3 && RandomNum <= 6)
                RandItemType = ItemType.WineHigh;
            else if (RandomNum >= 6 && RandomNum <= 9)
                RandItemType = ItemType.MeatHigh;
            else if (RandomNum >= 9 && RandomNum <= 12)
                RandItemType = ItemType.Core_Boar;
            else if (RandomNum >= 12 && RandomNum <= 15)
                RandItemType = ItemType.Core_Badger;
            else if (RandomNum >= 15 && RandomNum <= 18)
                RandItemType = ItemType.Core_Owl;
            else if (RandomNum >= 18 && RandomNum <= 21)
                RandItemType = ItemType.Core_Wolf;
            else if (RandomNum >= 21 && RandomNum <= 24)
                RandItemType = ItemType.Core_Fox;
            else if (RandomNum >= 24 && RandomNum <= 33.5)
                RandItemType = ItemType.CheeseMed;
            else if (RandomNum >= 33.5 && RandomNum <= 43)
                RandItemType = ItemType.WineMed;
            else if (RandomNum >= 43 && RandomNum <= 52.5)
                RandItemType = ItemType.MeatMed;
            else if (RandomNum >= 52.5 && RandomNum <= 62)
                RandItemType = ItemType.Core_Ferret;
            else if (RandomNum >= 62 && RandomNum <= 71.5)
                RandItemType = ItemType.Core_Falcon;
            else if (RandomNum >= 71.5 && RandomNum <= 81)
                RandItemType = ItemType.Core_Skunk;
            else if (RandomNum >= 81 && RandomNum <= 90.5)
                RandItemType = ItemType.Core_Chameleon;
            else if (RandomNum >= 90.5 && RandomNum <= 100)
                RandItemType = ItemType.Core_Snake;

            if (RandomItemDic.ContainsKey(RandItemType))
            {
                switch (RandItemType)
                {
                    case ItemType.CheeseHigh:
                    case ItemType.WineHigh:
                    case ItemType.MeatHigh:
                    case ItemType.Core_Boar:
                    case ItemType.Core_Badger:
                    case ItemType.Core_Owl:
                    case ItemType.Core_Wolf:
                    case ItemType.Core_Fox:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 3);
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 7);
                        break;
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 10);
                        break;
                }
            }
            else
            {
                switch (RandItemType)
                {
                    case ItemType.CheeseHigh:
                    case ItemType.WineHigh:
                    case ItemType.MeatHigh:
                    case ItemType.Core_Boar:
                    case ItemType.Core_Badger:
                    case ItemType.Core_Owl:
                    case ItemType.Core_Wolf:
                    case ItemType.Core_Fox:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 3)));
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 7)));
                        break;
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 10)));
                        break;

                }
                RandomItemDic[RandItemType].ItemType = RandItemType;
                RandomItemDic[RandItemType].ItemImg = TempUserElements.ItemDataList[(int)RandItemType].ItemImg;
                RandomItemDic[RandItemType].ItemImgTint = TempUserElements.ItemDataList[(int)RandItemType].ItemImgTint;
                RandomItemDic[RandItemType].ShopTitle = TempUserElements.ItemDataList[(int)RandItemType].ItemName;
                RandomItemDic[RandItemType].PopupDesc = TempUserElements.ItemDataList[(int)RandItemType].ItemDesc;
            }
        }

        foreach (var RandomItem in RandomItemDic)
        {
            TempUserElements.AddItem(RandomItem.Key, RandomItem.Value.ItemCount);
            EndGameRewardList.Add(RandomItem.Value);
        }
        EndGameRewardList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });

        return EndGameRewardList;
    }

    List<ShopItemData> LoserBox()
    {
        EndGameRewardList.Clear();

        Dictionary<ItemType, ShopItemData> RandomItemDic = new Dictionary<ItemType, ShopItemData>();
        UserElements TempUserElements = SceneStarter.Instance.userElements;

        ItemType RandItemType = ItemType.None;


        for (int i = 0; i < 5; ++i)
        {
            float RandomNum = UnityEngine.Random.Range(0.0f, 100.0f);

            if (RandomNum >= 0 && RandomNum <= 1)               // 고급치즈 
                RandItemType = ItemType.CheeseHigh;
            else if (RandomNum >= 1 && RandomNum <= 2)          // 고급와인
                RandItemType = ItemType.WineHigh;
            else if (RandomNum >= 2 && RandomNum <= 3)          // 고급고기
                RandItemType = ItemType.MeatHigh;
            else if (RandomNum >= 3 && RandomNum <= 6)          // 2티어 코어 
                RandItemType = ItemType.Core_Ferret;
            else if (RandomNum >= 6 && RandomNum <= 9)          // 2티어 코어
                RandItemType = ItemType.Core_Falcon;
            else if (RandomNum >= 9 && RandomNum <= 12)         // 2티어 코어
                RandItemType = ItemType.Core_Skunk;
            else if (RandomNum >= 12 && RandomNum <= 15)        // 2티어 코어
                RandItemType = ItemType.Core_Chameleon;
            else if (RandomNum >= 15 && RandomNum <= 18)        // 2티어 코어
                RandItemType = ItemType.Core_Snake;
            else if (RandomNum >= 18 && RandomNum <= 21)        // 중급치즈
                RandItemType = ItemType.CheeseMed;
            else if (RandomNum >= 21 && RandomNum <= 24)        // 중급와인
                RandItemType = ItemType.WineMed;
            else if (RandomNum >= 24 && RandomNum <= 27)        // 중급고기
                RandItemType = ItemType.MeatMed;
            else if (RandomNum >= 27 && RandomNum <= 36)        // 1티어 코어 
                RandItemType = ItemType.Core_Squirrel;
            else if (RandomNum >= 36 && RandomNum <= 45)        // 1티어 코어
                RandItemType = ItemType.Core_Lizard;
            else if (RandomNum >= 45 && RandomNum <= 54)        // 1티어 코어
                RandItemType = ItemType.Core_Toad;
            else if (RandomNum >= 54 && RandomNum <= 63)        // 1티어 코어
                RandItemType = ItemType.Core_Pigeon;
            else if (RandomNum >= 63 && RandomNum <= 72)        // 1티어 코어
                RandItemType = ItemType.Core_Mole;
            else if (RandomNum >= 72 && RandomNum <= 81.3f)        // 쓰레기 치즈
                RandItemType = ItemType.CheeseLow;
            else if (RandomNum >= 81.3f && RandomNum <= 90.6f)        // 쓰레기 와인
                RandItemType = ItemType.WineLow;
            else if (RandomNum >= 90.6f && RandomNum <= 100)        // 쓰레기 고기
                RandItemType = ItemType.MeatLow;

            if (RandomItemDic.ContainsKey(RandItemType))
            {
                switch (RandItemType)
                {
                    case ItemType.CheeseHigh:
                    case ItemType.WineHigh:
                    case ItemType.MeatHigh:
                        RandomItemDic[RandItemType].ItemCount += 1;
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 4);
                        break;
                    case ItemType.CheeseLow:
                    case ItemType.WineLow:
                    case ItemType.MeatLow:
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 7);
                        break;
                }

            }
            else
            {
                switch (RandItemType)
                {
                    case ItemType.CheeseHigh:
                    case ItemType.WineHigh:
                    case ItemType.MeatHigh:
                        RandomItemDic.Add(RandItemType, new ShopItemData(1));
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 4)));
                        break;
                    case ItemType.CheeseLow:
                    case ItemType.WineLow:
                    case ItemType.MeatLow:
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 7)));
                        break;
                }
                RandomItemDic[RandItemType].ItemType = RandItemType;
                RandomItemDic[RandItemType].ItemImg = TempUserElements.ItemDataList[(int)RandItemType].ItemImg;
                RandomItemDic[RandItemType].ItemImgTint = TempUserElements.ItemDataList[(int)RandItemType].ItemImgTint;
                RandomItemDic[RandItemType].ShopTitle = TempUserElements.ItemDataList[(int)RandItemType].ItemName;
                RandomItemDic[RandItemType].PopupDesc = TempUserElements.ItemDataList[(int)RandItemType].ItemDesc;
            }
        }

        foreach (var RandomItem in RandomItemDic)
        {
            TempUserElements.AddItem(RandomItem.Key, RandomItem.Value.ItemCount);
            EndGameRewardList.Add(RandomItem.Value);
        }
        EndGameRewardList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });

        return EndGameRewardList;
    }

    public void Vibrate()
    {
        if (SceneStarter.Instance.userElements.optionData.vibeOn)
            Handheld.Vibrate();
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
                return null;
            else
                return instance;
        }
    }
}
