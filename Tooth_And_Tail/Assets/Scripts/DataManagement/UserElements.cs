using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-09-27
///  
///  Desc.
///     UserData를 초기화 및 세이브,로드 하는 코드
///     미션데이터,아이템데이터,유닛소유여부,유닛레벨 데이터 또한 관리
/// 
/// </summary>

public enum ItemType
{
    None,
    CheeseLow, CheeseMed, CheeseHigh,                                   // 기본 강화재료
    WineLow, WineMed, WineHigh,
    MeatLow, MeatMed, MeatHigh,
    Core_Squirrel, Core_Lizard, Core_Toad, Core_Pigeon, Core_Mole,      // Core_Squirrel == 10 ,특수 강화재료 (Lv.3, 6, 9 강화때 필요)
    Core_Ferret, Core_Falcon, Core_Skunk, Core_Chameleon, Core_Snake,
    Core_Boar, Core_Badger, Core_Owl, Core_Wolf, Core_Fox,
    StaminaPortion, GoldPortion, JewelPortion,                             // 재화 회복

    ChangeNicNameTicket = 100, Booster,
    RandomCore = 250,
    RandomBox1 = 300, RandomBox2
}

// 미션 종류(일간,주간,업적)
public enum MissionType
{
    None, Daily, Weekly, Achievements
}

public enum ShopType
{
    Goods, Growth, BuffGoods
}

public enum PriceType
{
    Gold, Jewel
}

public enum PurchaseType
{
    Daily, Weekly, Normal
}

[System.Serializable]
public class UserData
{
    public string UserName;
    public int UserLevel;
    public int UserCurExp;
    public int UserMaxExp;
    public int UserCurStamina;
    public int UserMaxStamina;
    public int UserGold;
    public int UserDia;
    // 챕터 깬 여부를 저장하는 리스트
    public List<bool> Chapter = new List<bool>();
    // 부스터 버프 적용 여부
    public bool bIsBooster = false;
}

[System.Serializable]
public class MissionData
{
    public MissionType missionType;
    public string MissionName;
    public string Missiondesc;
    public int MissionCount;
    public int CompleteCount;
    public bool bIsComplete;
    public List<RewardData> RewardList;

    public MissionData()
    {

    }

    public MissionData(MissionData rhs)
    {
        missionType = rhs.missionType;
        MissionName = rhs.MissionName;
        Missiondesc = rhs.Missiondesc;
        MissionCount = rhs.MissionCount;
        CompleteCount = rhs.CompleteCount;
        bIsComplete = rhs.bIsComplete;
        RewardList = new List<RewardData>(rhs.RewardList);
    }
}

[System.Serializable]
public class LinkMissionData
{
    public List<MissionData> LinkMission;
}

[System.Serializable]
public class RewardData
{
    public ItemType ItemType;
    public int ItemCount;

    public RewardData(ItemType ItemType, int ItemCount)
    {
        this.ItemType = ItemType;
        this.ItemCount = ItemCount;
    }

    public RewardData(RewardData rhs)
    {
        ItemType = rhs.ItemType;
        ItemCount = rhs.ItemCount;
    }
}

[System.Serializable]
public class ItemData
{
    public ItemType ItemType;            // 아이템 종류
    public string ItemName;              // 아이템 이름
    public string ItemSentence;          // 아이템 한줄 설명
    public string ItemDesc;              // 아이템 설명
    public Sprite ItemImg;               // 아이템 이미지
    public Sprite ItemImgTint;           // 아이템 이미지 틴트
    public int ItemCount;                // 아이템 현재 개수
    public int ItemMaxCount;             // 아이템 최대 개수
    public int price;                    // 가격
    public int ItemEffect;               // 아이템 효과

    public ItemData(ItemData rhs)
    {
        ItemType = rhs.ItemType;
        ItemName = rhs.ItemName;
        ItemSentence = rhs.ItemSentence;
        ItemDesc = rhs.ItemDesc;
        ItemImg = rhs.ItemImg;
        ItemImgTint = rhs.ItemImgTint;
        ItemCount = rhs.ItemCount;
        ItemMaxCount = rhs.ItemMaxCount;
        price = rhs.price;
        ItemEffect = rhs.ItemEffect;
    }
}

[System.Serializable]
public class ShopItemData
{
    public PurchaseType PurchaseType;    // 아이템 구매제한(주간,일간,통상)
    public ItemType ItemType;            // 아이템 종류
    public string ShopTitle;             // 상점 아이템 이름
    public string ShopDesc;              // 상점 아이템 설명
    public string PopupDesc;             // 팝업 내용
    public Sprite ItemImg;               // 아이템 이미지
    public Sprite ItemImgTint;           // 아이템 이미지 틴트
    public int LeftCount;                // 아이템 현재 개수
    public int LimitCount;               // 아이템 최대 구매 개수
    public PriceType PriceType;          // 가격 타입
    public int Price;                    // 가격
    public int ItemCount;                // 아이템 효과
    public bool bIsSoldout;              // 품절 여부
    public bool bIsRandomGoods;          // 랜덤 상품 여부

    public ShopItemData()
    {
        PurchaseType = PurchaseType.Daily;
        ItemType = ItemType.None;
    }

    public ShopItemData(int ItemCount)
    {
        this.ItemCount = ItemCount;
    }

    public ShopItemData(ShopItemData rhs)
    {
        PurchaseType = rhs.PurchaseType;
        ItemType = rhs.ItemType;
        ShopTitle = rhs.ShopTitle;
        ShopDesc = rhs.ShopDesc;
        ItemImg = rhs.ItemImg;
        ItemImgTint = rhs.ItemImgTint;
        LeftCount = rhs.LeftCount;
        LimitCount = rhs.LimitCount;
        PriceType = rhs.PriceType;
        Price = rhs.Price;
        ItemCount = rhs.ItemCount;
    }
}

[System.Serializable]
public class UnitData
{
    public CommonType Type;
    public bool IsPossession;
    public int Level;
}

[System.Serializable]
public class OptionData
{
    public bool pushOn;
    public bool vibeOn;
    public float soundBG;
    public float soundAB;
    public float soundEF;
    public float soundVo;
}

public class UserElements : ScriptableObject
{
    // 초기화 Data
    public UserData FirstUserData = new UserData();
    // 저장 Data
    public UserData UserData = new UserData();
    // 저장 OptionData
    public OptionData optionData = new OptionData();
    // 챕터 갯수
    int ChapterCount = 2;

    // Load할지 결정하는 bool값
    public bool IsLoad = true;

    // 튜토리얼 미션데이터
    public List<MissionData> TutorialMissions = new List<MissionData>();
    // 일간 미션데이터
    public List<MissionData> DailyMissions = new List<MissionData>();
    // 주간 미션 데이터
    public List<MissionData> WeeklyMissions = new List<MissionData>();
    // 업적 데이터
    public List<LinkMissionData> Achievements = new List<LinkMissionData>();

    // 미션 데이터들을 모아둔 Dictionary
    public Dictionary<MissionType, List<MissionData>> MissionsDic = new Dictionary<MissionType, List<MissionData>>();

    // 아이템 데이터
    [ArrayElementTitle("ItemType")]
    public List<ItemData> ItemDataList = new List<ItemData>();

    // 재화 상점 데이터
    public List<ShopItemData> GoodsShopDataList = new List<ShopItemData>();
    // 성장 상점 데이터
    public List<ShopItemData> GrowthShopDataList = new List<ShopItemData>();
    // 버프 상점 데이터
    public List<ShopItemData> BuffGoodsShopDataList = new List<ShopItemData>();

    // 상점 타입에 따라 상품들 리스트를 넣어둔 딕셔너리
    public Dictionary<ShopType, List<ShopItemData>> ShopItemDic = new Dictionary<ShopType, List<ShopItemData>>();

    // 부스터 적용되는 첫번째 시간
    public DateTime FirstBoosterTime;
    // 부스터 남은 시간
    public TimeSpan RestBoosterTime;
    // 마지막 로그인 시간
    public DateTime LastLoginTime;
    // 스태미나 충전 시간
    public TimeSpan ChargeStaminaTime = TimeSpan.Zero;

    // 랜덤 박스1 보상 목록
    public List<ShopItemData> RandomBox1RewardList;
    // 랜덤 박스2 보상 목록
    public List<ShopItemData> RandomBox2RewardList;

    // 랜덤 박스에서 나온 결과물 저장
    public List<ShopItemData> RandomBoxItemList = new List<ShopItemData>();

    // 유닛 소유 여부 및 레벨 저장하는 리스트
    [ArrayElementTitle("Type")]
    public List<UnitData> UnitDataList = new List<UnitData>();

    // 유닛 최대 레벨
    int Max_Level = 10;

    // 레벨업 여부
    public bool bLevelUp = false;

    public void Refresh()
    {
        string[] CSVMissionDataArr = ExMethods.CSVReader("Data/CSV/MissionData");

        TutorialMissions.Clear();
        //DailyMissions.Clear();
        //WeeklyMissions.Clear();

        for (int i = 1; i < CSVMissionDataArr.Length; ++i)
        {
            string[] CSVMissionData = CSVMissionDataArr[i].Split(',');

            MissionData InsertData = new MissionData();

            switch (CSVMissionData[0].ToEnum<MissionType>())
            {
                case (int)MissionType.None:
                    InsertData.missionType = MissionType.None;
                    InsertData.MissionName = CSVMissionData[2];
                    InsertData.Missiondesc = CSVMissionData[3];
                    InsertData.MissionCount = int.Parse(CSVMissionData[4]);
                    InsertData.CompleteCount = int.Parse(CSVMissionData[5]);
                    TutorialMissions.Add(InsertData);
                    break;
                    //case (int)MissionType.Daily:
                    //    InsertData.missionType = MissionType.Daily;
                    //    InsertData.MissionName = CSVMissionData[2];
                    //    InsertData.Missiondesc = CSVMissionData[3];
                    //    InsertData.MissionCount = int.Parse(CSVMissionData[4]);
                    //    InsertData.CompleteCount = int.Parse(CSVMissionData[5]);
                    //    DailyMissions.Add(InsertData);
                    //    break;
                    //case (int)MissionType.Weekly:
                    //    InsertData.missionType = MissionType.Weekly;
                    //    InsertData.MissionName = CSVMissionData[2];
                    //    InsertData.Missiondesc = CSVMissionData[3];
                    //    InsertData.MissionCount = int.Parse(CSVMissionData[4]);
                    //    InsertData.CompleteCount = int.Parse(CSVMissionData[5]);
                    //    WeeklyMissions.Add(InsertData);
                    //    break;
                    //case (int)MissionType.Achievements:
                    //    break;
            }
        }
    }

    // 초기화
    public void InitializeElement()
    {
        for (int i = (int)MissionType.None; i <= (int)MissionType.Achievements; ++i)
        {
            MissionsDic.Add((MissionType)i, new List<MissionData>());
        }

        UserData.Chapter.Clear();

        SetMissions();
        // 로드실패 or bool값이 false라면 실행
        if (!IsLoad || !Load())
        {
            Debug.Log("데이터 파일이 없습니다");
            Debug.Log("새로운 계정을 만듭니다");

            UserData.UserName = FirstUserData.UserName;
            UserData.UserLevel = FirstUserData.UserLevel;
            UserData.UserCurExp = FirstUserData.UserCurExp;
            UserData.UserMaxExp = FirstUserData.UserMaxExp;
            UserData.UserCurStamina = FirstUserData.UserCurStamina;
            UserData.UserMaxStamina = FirstUserData.UserMaxStamina;
            UserData.UserGold = FirstUserData.UserGold;
            UserData.UserDia = FirstUserData.UserDia;

            for (int i = 0; i < ChapterCount; ++i)
                UserData.Chapter.Add(false);

            for (int i = 0; i < ItemDataList.Count; ++i)
            {
                ItemDataList[i].ItemCount = 0;
            }

            UserData.bIsBooster = false;
            InitializeShopItemList();
            InitializeOptionData();

            //유닛 레벨 1로 초기화
            for (int i = 0; i < UnitDataList.Count; ++i)
                UnitDataList[i].Level = 1;
        }
    }

    // OptionData 초기화
    public void InitializeOptionData()
    {
        optionData.pushOn = true;
        optionData.vibeOn = true;
        optionData.soundAB = 1;
        optionData.soundBG = 1;
        optionData.soundEF = 1;
        optionData.soundVo = 1;
    }

    // 유저 데이터 로컬영역에 저장
    public bool Save()
    {
        // 기본 정보 유저데이터들
        string toJson = JsonUtility.ToJson(UserData);
        // 옵션 데이터
        string toOptionJson = JsonUtility.ToJson(optionData);
        // 미션 데이터들
        string toDailyMissionListJson = JsonHelper.ToJson(MissionsDic[MissionType.Daily].ToArray());
        string toWeeklyMissionListJson = JsonHelper.ToJson(MissionsDic[MissionType.Weekly].ToArray());
        string toAchievementsListJson = JsonHelper.ToJson(MissionsDic[MissionType.Achievements].ToArray());
        // 상점 데이터들
        string toGoodsShopListJson = JsonHelper.ToJson(ShopItemDic[ShopType.Goods].ToArray());
        string toGrowthShopListJson = JsonHelper.ToJson(ShopItemDic[ShopType.Growth].ToArray());
        string toBuffGoodsShopListJson = JsonHelper.ToJson(ShopItemDic[ShopType.BuffGoods].ToArray());
        // 인벤토리 아이템 데이터들
        string toItemListJson = JsonHelper.ToJson(ItemDataList.ToArray());
        // 유닛 데이터들
        string toUnitListJson = JsonHelper.ToJson(UnitDataList.ToArray());
        // 첫번째 부스터 시간
        string toFirstBoosterTimeJson = FirstBoosterTime.ToString("yyyyMMddHHmmss");
        // 남은 부스터 시간
        string toRestBoosterTimeJson = RestBoosterTime.ToString("c");
        // 마지막 로그인 시간(종료시간)
        LastLoginTime = DateTime.Now;
        string toLastLoginTimeJson = LastLoginTime.ToString("yyyyMMddHHmmss");

        // 폴더가 없으면 폴더를 생성
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        // 유저 데이터 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/data.json", toJson);
        // 옵션 데이터 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/OptionData.json", toOptionJson);
        // 미션 데이터들 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/DailyMissionData.json", toDailyMissionListJson);
        File.WriteAllText(Application.persistentDataPath + "/Saves/WeeklyMissionData.json", toWeeklyMissionListJson);
        File.WriteAllText(Application.persistentDataPath + "/Saves/Achievements.json", toAchievementsListJson);
        // 상점 데이터들 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/GoodsShopData.json", toGoodsShopListJson);
        File.WriteAllText(Application.persistentDataPath + "/Saves/GrowthShopData.json", toGrowthShopListJson);
        File.WriteAllText(Application.persistentDataPath + "/Saves/BuffGoodsShopData.json", toBuffGoodsShopListJson);
        // 인벤토리 데이터 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/ItemData.json", toItemListJson);
        // 유닛 데이터 저장
        File.WriteAllText(Application.persistentDataPath + "/Saves/UnitData.json", toUnitListJson);
        // 첫번째 부스터 시간
        File.WriteAllText(Application.persistentDataPath + "/Saves/FirstBoosterTime.json", toFirstBoosterTimeJson);
        // 남은 부스터 시간
        File.WriteAllText(Application.persistentDataPath + "/Saves/RestBoosterTime.json", toRestBoosterTimeJson);
        // 마지막 로그인 시간
        File.WriteAllText(Application.persistentDataPath + "/Saves/LastLoginTime.json", toLastLoginTimeJson);

        return true;
    }

    // 로드시 로컬영역에 저장된 데이터를 가져옴
    bool Load()
    {
        // 파일이 존재하지않으면 return
        if (!File.Exists(Application.persistentDataPath + "/Saves/data.json") &&
            !File.Exists(Application.persistentDataPath + "/Saves/DailyMissionData.json") &&
            !File.Exists(Application.persistentDataPath + "/Saves/GoodsShopData.json"))
        {
            return false;
        }
        // 유저 데이터 로드    
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/data.json");
        UserData = JsonUtility.FromJson<UserData>(jsonString);

        // 옵션 데이터 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/OptionData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/OptionData.json");
            optionData = JsonUtility.FromJson<OptionData>(jsonString);
        }

        // 미션 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/DailyMissionData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/DailyMissionData.json");
            MissionsDic[MissionType.Daily].Clear();
            foreach (var array in JsonHelper.FromJsonArray<MissionData>(jsonString))
            {
                MissionsDic[MissionType.Daily].Add(array);
            }
        }
        if (File.Exists(Application.persistentDataPath + "/Saves/WeeklyMissionData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/WeeklyMissionData.json");
            MissionsDic[MissionType.Weekly].Clear();
            foreach (var array in JsonHelper.FromJsonArray<MissionData>(jsonString))
            {
                MissionsDic[MissionType.Weekly].Add(array);
            }
        }
        if (File.Exists(Application.persistentDataPath + "/Saves/Achievements.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/Achievements.json");
            MissionsDic[MissionType.Achievements].Clear();
            foreach (var array in JsonHelper.FromJsonArray<MissionData>(jsonString))
            {
                MissionsDic[MissionType.Achievements].Add(array);
            }
        }

        // 상점 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/GoodsShopData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/GoodsShopData.json");
            ShopItemDic.Add(ShopType.Goods, new List<ShopItemData>());
            foreach (var array in JsonHelper.FromJsonArray<ShopItemData>(jsonString))
            {
                ShopItemDic[ShopType.Goods].Add(array);
            }
        }
        if (File.Exists(Application.persistentDataPath + "/Saves/GrowthShopData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/GrowthShopData.json");
            ShopItemDic.Add(ShopType.Growth, new List<ShopItemData>());
            foreach (var array in JsonHelper.FromJsonArray<ShopItemData>(jsonString))
            {
                ShopItemDic[ShopType.Growth].Add(array);
            }
        }
        if (File.Exists(Application.persistentDataPath + "/Saves/BuffGoodsShopData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/BuffGoodsShopData.json");
            ShopItemDic.Add(ShopType.BuffGoods, new List<ShopItemData>());
            foreach (var array in JsonHelper.FromJsonArray<ShopItemData>(jsonString))
            {
                ShopItemDic[ShopType.BuffGoods].Add(array);
            }
        }

        // 인벤토리 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/ItemData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/ItemData.json");
            ItemDataList.Clear();
            foreach (var array in JsonHelper.FromJsonArray<ItemData>(jsonString))
            {
                ItemDataList.Add(array);
            }
        }

        // 유닛 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/UnitData.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/UnitData.json");
            UnitDataList.Clear();
            foreach (var array in JsonHelper.FromJsonArray<UnitData>(jsonString))
            {
                UnitDataList.Add(array);
            }
        }

        // 시간 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/FirstBoosterTime.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/FirstBoosterTime.json");
            FirstBoosterTime = DateTime.ParseExact(jsonString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        // 시간 데이터가 있으면 로드
        if (File.Exists(Application.persistentDataPath + "/Saves/RestBoosterTime.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/RestBoosterTime.json");
            RestBoosterTime = TimeSpan.ParseExact(jsonString, "c", CultureInfo.InvariantCulture);
        }

        if (File.Exists(Application.persistentDataPath + "/Saves/LastLoginTime.json"))
        {
            jsonString = File.ReadAllText(Application.persistentDataPath + "/Saves/LastLoginTime.json");
            LastLoginTime = DateTime.ParseExact(jsonString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        CalculationStamina();
        CheckReset();

        return true;
    }

    // 아이템 갯수가 0인지 판별
    public bool IsItemEmpty(ItemType itemType)
    {
        int iType = (int)itemType;
        return ItemDataList[iType].ItemCount == 0;
    }

    public bool IsItemEmpty(int itemTypeIndex)
    {
        return ItemDataList[itemTypeIndex].ItemCount == 0;
    }

    // 아이템 갯수 가져오는 함수
    public int GetItemCount(ItemType itemType)
    {
        int iType = (int)itemType;
        return ItemDataList[iType].ItemCount;
    }

    public int GetItemCount(int itemTypeIndex)
    {
        int iType = itemTypeIndex;
        return ItemDataList[iType].ItemCount;
    }

    // 코어 갯수를 가져오는 함수(유닛타입)
    public int GetCoreCount(CommonType UnitType)
    {
        int iType = (int)UnitType + 10;
        return ItemDataList[iType].ItemCount;
    }
    // 코어 갯수를 가져오는 함수(유닛타입 인덱스)
    public int GetCoreCount(int UnitTypeIndex)
    {
        int iType = UnitTypeIndex + 10;
        return ItemDataList[iType].ItemCount;
    }

    // 코어를 갯수만큼 사용하는 함수(유닛타입)
    public bool UseCoreCount(CommonType UnitType, int UseCount)
    {
        int iType = (int)UnitType + 10;
        if (ItemDataList[iType].ItemCount >= UseCount)
        {
            ItemDataList[iType].ItemCount -= UseCount;
            return true;
        }
        else
            return false;
    }
    // 코어를 갯수만큼 사용하는 함수(유닛타입 인덱스)
    public bool UseCoreCount(int UnitTypeIndex, int UseCount)
    {
        if (ItemDataList[UnitTypeIndex + 10].ItemCount >= UseCount)
        {
            ItemDataList[UnitTypeIndex + 10].ItemCount -= UseCount;
            return true;
        }
        else
            return false;
    }

    // 아이템 갯수를 추가하는 함수
    public void AddItem(ItemType itemType, int AddCount)
    {
        int iType = (int)itemType;
        ItemDataList[iType].ItemCount += AddCount;
        if (ItemDataList[iType].ItemCount >= ItemDataList[iType].ItemMaxCount)
            ItemDataList[iType].ItemCount = ItemDataList[iType].ItemMaxCount;
    }

    // 아이템 갯수를 추가하는 함수
    public void AddItem(int itemTypeIndex, int AddCount)
    {
        ItemDataList[itemTypeIndex].ItemCount += AddCount;
        if (ItemDataList[itemTypeIndex].ItemCount >= ItemDataList[itemTypeIndex].ItemMaxCount)
            ItemDataList[itemTypeIndex].ItemCount = ItemDataList[itemTypeIndex].ItemMaxCount;
    }

    // 아이템 사용하는 함수
    public bool UseItem(ItemType itemType, int UseCount)
    {
        int iType = (int)itemType;
        if (ItemDataList[iType].ItemCount >= UseCount)
        {
            switch (itemType)
            {
                case ItemType.StaminaPortion:
                    UserData.UserCurStamina += UseCount * Global.StaminaPortionEffect;
                    ItemDataList[(int)itemType].ItemCount -= UseCount;
                    break;
                case ItemType.GoldPortion:
                    UserData.UserGold += UseCount * Global.GoldPortionEffect;
                    ItemDataList[(int)itemType].ItemCount -= UseCount;
                    break;
                case ItemType.JewelPortion:
                    UserData.UserDia += UseCount * Global.JewelPortionEffect;
                    ItemDataList[(int)itemType].ItemCount -= UseCount;
                    break;
                default:
                    ItemDataList[iType].ItemCount -= UseCount;
                    break;
            }
            return true;
        }
        else
            return false;
    }

    // 아이템 사용하는 함수
    public bool UseItem(int itemTypeIndex, int UseCount)
    {
        if (ItemDataList[itemTypeIndex].ItemCount >= UseCount)
        {
            switch (itemTypeIndex)
            {
                case (int)ItemType.StaminaPortion:
                    UserData.UserCurStamina += UseCount * Global.StaminaPortionEffect;
                    ItemDataList[itemTypeIndex].ItemCount -= UseCount;
                    break;
                case (int)ItemType.GoldPortion:
                    UserData.UserGold += UseCount * Global.GoldPortionEffect;
                    ItemDataList[itemTypeIndex].ItemCount -= UseCount;
                    break;
                case (int)ItemType.JewelPortion:
                    UserData.UserDia += UseCount * Global.JewelPortionEffect;
                    ItemDataList[itemTypeIndex].ItemCount -= UseCount;
                    break;
                default:
                    ItemDataList[itemTypeIndex].ItemCount -= UseCount;
                    break;
            }
            return true;
        }
        else
            return false;
    }

    public bool UseAllitem(ItemType itemType)
    {
        if (ItemDataList[(int)itemType].ItemCount > 0)
        {
            switch (itemType)
            {
                case ItemType.StaminaPortion:
                    UserData.UserCurStamina += ItemDataList[(int)itemType].ItemCount * Global.StaminaPortionEffect;
                    ItemDataList[(int)itemType].ItemCount = 0;
                    break;
                case ItemType.GoldPortion:
                    UserData.UserGold += ItemDataList[(int)itemType].ItemCount * Global.GoldPortionEffect;
                    ItemDataList[(int)itemType].ItemCount = 0;
                    break;
                case ItemType.JewelPortion:
                    UserData.UserDia += ItemDataList[(int)itemType].ItemCount * Global.JewelPortionEffect;
                    ItemDataList[(int)itemType].ItemCount = 0;
                    break;
                default:
                    ItemDataList[(int)itemType].ItemCount = 0;
                    break;
            }
            return true;
        }
        else
            return false;
    }

    // 아이템 판매하는 함수
    public bool SellItem(ItemType itemType, int SellCount)
    {
        int iType = (int)itemType;
        if (ItemDataList[iType].ItemCount >= SellCount)
        {
            ItemDataList[iType].ItemCount -= SellCount;
            UserData.UserGold += ItemDataList[iType].price * SellCount;
            return true;
        }
        return false;
    }

    // 아이템 판매하는 함수
    public bool SellItem(int itemTypeIndex, int SellCount)
    {
        if (ItemDataList[itemTypeIndex].ItemCount >= SellCount)
        {
            ItemDataList[itemTypeIndex].ItemCount -= SellCount;
            UserData.UserGold += ItemDataList[itemTypeIndex].price * SellCount;
            return true;
        }
        return false;
    }

    // 챕터의 클리어 여부(진행도)를 셋팅하는 함수
    public void SetChapter(int Index, bool IsClear)
    {
        UserData.Chapter[Index] = IsClear;
    }

    // 유닛을 소유 여부를 셋팅하는 함수
    public void SetPossession(CommonType Type, bool IsPossession)
    {
        UnitDataList[(int)Type].IsPossession = IsPossession;
    }
    // 유닛을 소유 여부를 셋팅하는 함수
    public void SetPossession(int Index, bool IsPossession)
    {
        UnitDataList[Index].IsPossession = IsPossession;
    }

    // 유닛의 레벨을 셋팅하는 함수(Max_Level == 10)
    public bool SetLevel(CommonType UnitType, int NewLevel)
    {
        if (NewLevel > Max_Level)
            return false;

        UnitDataList[(int)UnitType].Level = NewLevel;
        return true;
    }
    // 유닛의 레벨을 셋팅하는 함수(Max_Level == 10)
    public bool SetLevel(int UnitTypeIndex, int NewLevel)
    {
        if (NewLevel > Max_Level)
            return false;

        UnitDataList[UnitTypeIndex].Level = NewLevel;
        return true;
    }

    // 유닛의 레벨을 업시키는 함수
    public bool LevelPlus(int UnitTypeIndex)
    {
        ++UnitDataList[UnitTypeIndex].Level;
        if (UnitDataList[UnitTypeIndex].Level > Max_Level)
        {
            UnitDataList[UnitTypeIndex].Level = Max_Level;
            return false;
        }

        return true;
    }
    // 유닛의 레벨을 업시키는 함수
    public bool LevelPlus(CommonType UnitType)
    {
        int iType = (int)UnitType;
        ++UnitDataList[iType].Level;
        if (UnitDataList[iType].Level > Max_Level)
        {
            UnitDataList[iType].Level = Max_Level;
            return false;
        }

        return true;
    }

    // 유닛 소유 여부를 알려주는 함수
    public bool GetIsPossession(CommonType UnitType)
    {
        return UnitDataList[(int)UnitType].IsPossession;
    }
    // 유닛 소유 여부를 알려주는 함수
    public bool GetIsPossession(int UnitTypeIndex)
    {
        return UnitDataList[UnitTypeIndex].IsPossession;
    }

    // 유닛의 레벨을 알려주는 함수
    public int GetLevel(CommonType UnitType)
    {
        return UnitDataList[(int)UnitType].Level;
    }
    // 유닛의 레벨을 알려주는 함수
    public int GetLevel(int UnitTypeIndex)
    {
        return UnitDataList[UnitTypeIndex].Level;
    }

    // 미션 데이터 초기화(Dictionary)
    public void SetMissions()
    {
        SetTutorialMissions();
        SetDailyMissions();
        SetWeeklyMissions();
        SetAchievements();
    }

    // 튜토리얼 미션 데이터 초기화
    public void SetTutorialMissions()
    {
        MissionsDic[MissionType.None].Clear();

        for (int i = 0; i < TutorialMissions.Count; ++i)
        {
            MissionsDic[MissionType.None].Add(new MissionData(TutorialMissions[i]));
        }
    }

    // 일간 미션 데이터 초기화
    public void SetDailyMissions()
    {
        MissionsDic[MissionType.Daily].Clear();

        for (int i = 0; i < DailyMissions.Count; ++i)
        {
            MissionsDic[MissionType.Daily].Add(new MissionData(DailyMissions[i]));
        }
    }

    // 주간 미션 데이터 초기화
    public void SetWeeklyMissions()
    {
        MissionsDic[MissionType.Weekly].Clear();

        for (int i = 0; i < WeeklyMissions.Count; ++i)
        {
            MissionsDic[MissionType.Weekly].Add(new MissionData(WeeklyMissions[i]));
        }
    }

    // 업적 데이터 초기화
    public void SetAchievements()
    {
        MissionsDic[MissionType.Achievements].Clear();

        for (int i = 0; i < Achievements.Count; ++i)
        {
            MissionsDic[MissionType.Achievements].Add(new MissionData(Achievements[i].LinkMission[0]));
        }
    }

    // 미션 완료함수 조건을 완료하면 보상을 지급
    public bool CompleteMission(MissionType missionType, int MissionIndex)
    {
        if (MissionsDic[missionType][MissionIndex].MissionCount >= MissionsDic[missionType][MissionIndex].CompleteCount)
        {
            MissionsDic[missionType][MissionIndex].MissionCount = MissionsDic[missionType][MissionIndex].CompleteCount;
            MissionsDic[missionType][MissionIndex].bIsComplete = true;

            foreach (var Reward in MissionsDic[missionType][MissionIndex].RewardList)
            {
                AddItem(Reward.ItemType, Reward.ItemCount);
            }
            return true;
        }

        return false;
    }

    // 미션리스트 가져오는 함수(미션타입 사용)
    public List<MissionData> GetMissionsList(MissionType missionType)
    {
        return MissionsDic[missionType];
    }

    // 미션리스트 가져오는 함수(미션타입의 인덱스 사용)
    public List<MissionData> GetMissionsList(int missionTypeIndex)
    {
        return MissionsDic[(MissionType)missionTypeIndex];
    }

    // 미션을 가져오는 함수(미션타입과 미션인덱스 사용)
    public MissionData GetMissionData(MissionType missionType, int MissionIndex)
    {
        return MissionsDic[missionType][MissionIndex];
    }

    // 미션을 가져오는 함수(미션타입의 인덱스와 미션인덱스 사용)
    public MissionData GetMissionData(int missionTypeIndex, int MissionIndex)
    {
        return MissionsDic[(MissionType)missionTypeIndex][MissionIndex];
    }

    // 미션의 보상을 가져오는 함수(미션타입과 미션인덱스 사용)
    public List<RewardData> GetRewardList(MissionType missionType, int MissionIndex)
    {
        return MissionsDic[missionType][MissionIndex].RewardList;
    }

    // 미션의 보상을 가져오는 함수(미션타입의 인덱스와 미션인덱스 사용)
    public List<RewardData> GetRewardList(int missionTypeIndex, int MissionIndex)
    {
        return MissionsDic[(MissionType)missionTypeIndex][MissionIndex].RewardList;
    }

    // 미션 완료 횟수를 설정하는 함수(미션타입과 미션인덱스 사용)
    public void SetMissionCount(MissionType missionType, int MissionIndex, int Count)
    {
        if (MissionsDic[missionType][MissionIndex].MissionCount == MissionsDic[missionType][MissionIndex].CompleteCount)
            return;

        MissionsDic[missionType][MissionIndex].MissionCount = Count;

        if (MissionsDic[missionType][MissionIndex].MissionCount >= MissionsDic[missionType][MissionIndex].CompleteCount)
        {
            MissionsDic[missionType][MissionIndex].MissionCount = MissionsDic[missionType][MissionIndex].CompleteCount;

            if (missionType >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
    }

    // 미션 완료 횟수를 설정하는 함수(미션타입의 인덱스와 미션인덱스 사용)
    public void SetMissionCount(int missionTypeIndex, int MissionIndex, int Count)
    {
        if (MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount == MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount)
            return;

        MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount = Count;

        if (MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount >= MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount)
        {
            MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount = MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount;

            if ((MissionType)missionTypeIndex >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
    }

    // 미션 완료 횟수를 추가하는 함수(미션타입과 미션인덱스 사용)
    public void AddMissionCount(MissionType missionType, int MissionIndex, int Count)
    {
        if (MissionsDic[missionType][MissionIndex].MissionCount == MissionsDic[missionType][MissionIndex].CompleteCount)
            return;

        MissionsDic[missionType][MissionIndex].MissionCount += Count;

        if (MissionsDic[missionType][MissionIndex].MissionCount >= MissionsDic[missionType][MissionIndex].CompleteCount)
        {
            MissionsDic[missionType][MissionIndex].MissionCount = MissionsDic[missionType][MissionIndex].CompleteCount;

            if(missionType >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
    }

    // 미션 완료 횟수를 추가하는 함수(미션타입의 인덱스와 미션인덱스 사용)
    public void AddMissionCount(int missionTypeIndex, int MissionIndex, int Count)
    {
        if (MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount == MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount)
            return;

        MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount += Count;

        if (MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount >= MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount)
        {
            MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount = MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount;

            if ((MissionType)missionTypeIndex >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
    }

    // 미션 완료 횟수를 1회 추가하는 함수(미션타입과 미션인덱스 사용)
    public void PlusMissionCount(MissionType missionType, int MissionIndex)
    {
        ++MissionsDic[missionType][MissionIndex].MissionCount;

        if (MissionsDic[missionType][MissionIndex].MissionCount >= MissionsDic[missionType][MissionIndex].CompleteCount)
        {
            MissionsDic[missionType][MissionIndex].MissionCount = MissionsDic[missionType][MissionIndex].CompleteCount;

            if (missionType >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
    }

    // 미션 완료 횟수를 1회 추가하는 함수(미션타입의 인덱스와 미션인덱스 사용)
    public void PlusMissionCount(int missionTypeIndex, int MissionIndex)
    {
        ++MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount;

        if (MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount >= MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount)
        {
            MissionsDic[(MissionType)missionTypeIndex][MissionIndex].MissionCount = MissionsDic[(MissionType)missionTypeIndex][MissionIndex].CompleteCount;

            if ((MissionType)missionTypeIndex >= MissionType.Daily)
                GameManager.Instance.CompleteMissionCount++;
        }
            
    }

    // 모든 상점 아이템 리스트 초기화
    public void InitializeShopItemList()
    {
        ShopItemDic.Clear();

        GoodsShopDataList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.PurchaseType.CompareTo(B.PurchaseType); });
        GrowthShopDataList.Sort(delegate (ShopItemData A, ShopItemData B)
        {
            if (A.PurchaseType == PurchaseType.Normal && B.PurchaseType == PurchaseType.Normal)
                return A.ItemType.CompareTo(B.ItemType);

            return A.PurchaseType.CompareTo(B.PurchaseType);
        });
        BuffGoodsShopDataList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.PurchaseType.CompareTo(B.PurchaseType); });

        for (int i = 0; i < GoodsShopDataList.Count; ++i)
        {
            switch (GoodsShopDataList[i].ItemType)
            {
                case ItemType.GoldPortion:
                    GoodsShopDataList[i].ShopDesc = "골드";
                    GoodsShopDataList[i].PopupDesc = "재화";
                    break;
                case ItemType.StaminaPortion:
                    GoodsShopDataList[i].ShopDesc = "탄환";
                    GoodsShopDataList[i].PopupDesc = "탄환 회복";
                    break;
            }
        }

        RandomBox1RewardList.Clear();
        RandomBox2RewardList.Clear();

        for (int i = 1; i < 10; ++i)
        {
            ShopItemData RandItemData = new ShopItemData();
            RandItemData.ShopTitle = ItemDataList[i].ItemName;
            RandItemData.ItemImg = ItemDataList[i].ItemImg;
            RandItemData.ItemImgTint = ItemDataList[i].ItemImgTint;
            RandItemData.ShopDesc = ItemDataList[i].ItemSentence;
            RandItemData.PopupDesc = ItemDataList[i].ItemDesc;
            RandomBox1RewardList.Add(RandItemData);
            if (i != 1 && i != 4 && i != 7)
                RandomBox2RewardList.Add(RandItemData);
        }

        for (int i = 0; i < GrowthShopDataList.Count; ++i)
        {
            if (GrowthShopDataList[i].PurchaseType == PurchaseType.Weekly && GrowthShopDataList[i].LimitCount == 3)
                GrowthShopDataList[i].ItemType = ItemType.RandomCore;

            if ((int)GrowthShopDataList[i].ItemType < 100)
            {
                GrowthShopDataList[i].ItemImg = ItemDataList[(int)GrowthShopDataList[i].ItemType].ItemImg;
                GrowthShopDataList[i].ItemImgTint = ItemDataList[(int)GrowthShopDataList[i].ItemType].ItemImgTint;
                GrowthShopDataList[i].ShopTitle = ItemDataList[(int)GrowthShopDataList[i].ItemType].ItemName;
                GrowthShopDataList[i].PopupDesc = ItemDataList[(int)GrowthShopDataList[i].ItemType].ItemDesc;
            }
            switch (GrowthShopDataList[i].ItemType)
            {
                case ItemType.Core_Squirrel:
                case ItemType.Core_Lizard:
                case ItemType.Core_Toad:
                case ItemType.Core_Pigeon:
                case ItemType.Core_Mole:
                    GrowthShopDataList[i].ShopDesc = "15개";
                    GrowthShopDataList[i].ItemCount = 15;
                    RandomBox1RewardList.Add(GrowthShopDataList[i]);
                    break;
                case ItemType.Core_Ferret:
                case ItemType.Core_Falcon:
                case ItemType.Core_Skunk:
                case ItemType.Core_Chameleon:
                case ItemType.Core_Snake:
                    GrowthShopDataList[i].ShopDesc = "10개";
                    GrowthShopDataList[i].ItemCount = 10;
                    RandomBox1RewardList.Add(GrowthShopDataList[i]);
                    RandomBox2RewardList.Add(GrowthShopDataList[i]);
                    break;
                case ItemType.Core_Boar:
                case ItemType.Core_Badger:
                case ItemType.Core_Owl:
                case ItemType.Core_Wolf:
                case ItemType.Core_Fox:
                    GrowthShopDataList[i].ShopDesc = "5개";
                    GrowthShopDataList[i].ItemCount = 5;
                    RandomBox2RewardList.Add(GrowthShopDataList[i]);
                    break;
            }
        }

        RandomBox1RewardList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });
        RandomBox2RewardList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });

        for (int i = 0; i < BuffGoodsShopDataList.Count; ++i)
        {
            if (BuffGoodsShopDataList[i].ItemType == ItemType.Booster)
            {
                BuffGoodsShopDataList[i].ShopDesc = BuffGoodsShopDataList[i].ItemCount + "시간";
                BuffGoodsShopDataList[i].PopupDesc = "경험치/골드 20% 추가획득";
            }
            else if (BuffGoodsShopDataList[i].ItemType == ItemType.ChangeNicNameTicket)
                BuffGoodsShopDataList[i].PopupDesc = "플레이어 닉네임 변경";
        }

        ShopItemDic.Add(ShopType.Goods, new List<ShopItemData>(GoodsShopDataList));
        ShopItemDic.Add(ShopType.Growth, new List<ShopItemData>(GrowthShopDataList));
        ShopItemDic.Add(ShopType.BuffGoods, new List<ShopItemData>(BuffGoodsShopDataList));

        ResetShopItemList();
        RandomUnitCore();
    }

    // 상점 아이템 리스트 초기화
    public void ResetShopItemList()
    {
        foreach (var ShopItemList in ShopItemDic)
        {
            foreach (var ShopItem in ShopItemList.Value)
            {
                ShopItem.LeftCount = ShopItem.LimitCount;
                ShopItem.bIsSoldout = false;
            }
        }
    }

    // 상점타입을 가져와서 그 타입에 맞는 상점 아이템 리스트 초기화
    public void ResetShopItemList(ShopType shopType)
    {
        foreach (var ShopItem in ShopItemDic[shopType])
        {
            ShopItem.LeftCount = ShopItem.LimitCount;
            ShopItem.bIsSoldout = false;
        }
    }

    // 상점타입을 가져와서 그 타입에 맞는 상점 아이템 리스트 초기화
    public void ResetShopItemList(ShopType shopType, PurchaseType purchaseType)
    {
        foreach (var ShopItem in ShopItemDic[shopType])
        {
            if (ShopItem.PurchaseType != purchaseType)
                continue;

            ShopItem.LeftCount = ShopItem.LimitCount;
            ShopItem.bIsSoldout = false;
        }
    }

    // 타입에 맞는 아이템 리스트 가져오기
    public List<ShopItemData> GetShopItemList(ShopType shopType)
    {
        return ShopItemDic[shopType];
    }

    // 타입과 상점 인덱스를 가져와서 아이템 데이터 가져오기
    public ShopItemData GetShopItem(ShopType shopType, int ShopItemIndex)
    {
        return ShopItemDic[shopType][ShopItemIndex];
    }

    // 타입과 상점 인덱스를 이용하여 조건을 만족하면 살 수 있게 하는 함수
    public bool BuyShopItem(ShopType shopType, int ShopItemIndex)
    {
        ShopItemData BuyItemData = ShopItemDic[shopType][ShopItemIndex];

        if (BuyItemData.bIsSoldout)
            return false;

        switch (shopType)
        {
            case ShopType.Goods:
                switch (BuyItemData.ItemType)
                {
                    case ItemType.StaminaPortion:
                        if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                        {
                            if (BuyItemData.LimitCount != 0)
                            {
                                --BuyItemData.LeftCount;
                                if (BuyItemData.LeftCount <= 0)
                                {
                                    BuyItemData.LeftCount = 0;
                                    BuyItemData.bIsSoldout = true;
                                }

                            }
                            SceneStarter.Instance.userElements.UserData.UserDia -= BuyItemData.Price;
                            SceneStarter.Instance.userElements.UserData.UserCurStamina += BuyItemData.ItemCount;
                            return true;
                        }
                        break;
                    case ItemType.GoldPortion:
                        if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                        {
                            if (BuyItemData.LimitCount != 0)
                            {
                                --BuyItemData.LeftCount;
                                if (BuyItemData.LeftCount <= 0)
                                {
                                    BuyItemData.LeftCount = 0;
                                    BuyItemData.bIsSoldout = true;
                                }
                            }

                            SceneStarter.Instance.userElements.UserData.UserDia -= BuyItemData.Price;
                            SceneStarter.Instance.userElements.UserData.UserGold += BuyItemData.ItemCount;
                            return true;
                        }
                        break;
                }
                break;
            case ShopType.Growth:
                switch (BuyItemData.ItemType)
                {
                    case ItemType.RandomBox1:
                        if (SceneStarter.Instance.userElements.UserData.UserGold >= BuyItemData.Price)
                        {
                            if (BuyItemData.LimitCount != 0)
                            {
                                --BuyItemData.LeftCount;
                                if (BuyItemData.LeftCount <= 0)
                                {
                                    BuyItemData.LeftCount = 0;
                                    BuyItemData.bIsSoldout = true;
                                }
                            }

                            SceneStarter.Instance.userElements.UserData.UserGold -= BuyItemData.Price;
                            ReinforceRandomBox1();
                            return true;
                        }
                        break;
                    case ItemType.RandomBox2:
                        if (SceneStarter.Instance.userElements.UserData.UserGold >= BuyItemData.Price)
                        {
                            if (BuyItemData.LimitCount != 0)
                            {
                                --BuyItemData.LeftCount;
                                if (BuyItemData.LeftCount <= 0)
                                {
                                    BuyItemData.LeftCount = 0;
                                    BuyItemData.bIsSoldout = true;
                                }
                            }

                            SceneStarter.Instance.userElements.UserData.UserGold -= BuyItemData.Price;
                            ReinforceRandomBox2();
                            return true;
                        }
                        break;
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                    case ItemType.Core_Boar:
                    case ItemType.Core_Badger:
                    case ItemType.Core_Owl:
                    case ItemType.Core_Wolf:
                    case ItemType.Core_Fox:
                        if (BuyItemData.PriceType == PriceType.Jewel)
                        {
                            if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                            {
                                if (BuyItemData.LimitCount != 0)
                                {
                                    --BuyItemData.LeftCount;
                                    if (BuyItemData.LeftCount <= 0)
                                    {
                                        BuyItemData.LeftCount = 0;
                                        BuyItemData.bIsSoldout = true;
                                    }
                                }

                                SceneStarter.Instance.userElements.UserData.UserDia -= BuyItemData.Price;
                                AddItem(BuyItemData.ItemType, BuyItemData.ItemCount);
                                return true;
                            }
                        }
                        else
                        {
                            if (SceneStarter.Instance.userElements.UserData.UserGold >= BuyItemData.Price)
                            {
                                if (BuyItemData.LimitCount != 0)
                                {
                                    --BuyItemData.LeftCount;
                                    if (BuyItemData.LeftCount <= 0)
                                    {
                                        BuyItemData.LeftCount = 0;
                                        BuyItemData.bIsSoldout = true;
                                    }
                                }

                                SceneStarter.Instance.userElements.UserData.UserGold -= BuyItemData.Price;
                                AddItem(BuyItemData.ItemType, BuyItemData.ItemCount);
                                return true;
                            }
                        }
                        break;
                }
                break;
            case ShopType.BuffGoods:
                switch (BuyItemData.ItemType)
                {
                    case ItemType.ChangeNicNameTicket:
                        if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                        {
                            SceneStarter.Instance.userElements.UserData.UserDia -= BuyItemData.Price;
                            return true;
                        }
                        break;
                    case ItemType.Booster:
                        if (BuyItemData.PriceType == PriceType.Jewel)
                        {
                            if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                            {
                                SceneStarter.Instance.userElements.UserData.UserDia -= BuyItemData.Price;
                                SetBoosterBuff(shopType, ShopItemIndex);
                                return true;
                            }
                        }
                        else
                        {
                            if (SceneStarter.Instance.userElements.UserData.UserGold >= BuyItemData.Price)
                            {
                                if (BuyItemData.LimitCount != 0)
                                {
                                    --BuyItemData.LeftCount;
                                    if (BuyItemData.LeftCount <= 0)
                                    {
                                        BuyItemData.LeftCount = 0;
                                        BuyItemData.bIsSoldout = true;
                                    }
                                }

                                SceneStarter.Instance.userElements.UserData.UserGold -= BuyItemData.Price;
                                SetBoosterBuff(shopType, ShopItemIndex);
                                return true;
                            }
                        }
                        break;
                }
                break;
        }
        return false;
    }

    // 상점 아이템을 살 만큼의 재화가 있는지 검사
    public bool BuyShopItemPossible(ShopType shopType, int ShopItemIndex)
    {
        ShopItemData BuyItemData = ShopItemDic[shopType][ShopItemIndex];

        if (BuyItemData.PriceType == PriceType.Jewel)
        {
            if (SceneStarter.Instance.userElements.UserData.UserDia >= BuyItemData.Price)
                return true;
        }
        else
        {
            if (SceneStarter.Instance.userElements.UserData.UserGold >= BuyItemData.Price)
                return true;
        }

        return false;
    }

    // 상점 유닛코어를 랜덤하게 설정하는 함수
    public void RandomUnitCore()
    {
        ItemType PreType = ItemType.None;

        foreach (var ShopItem in ShopItemDic[ShopType.Growth])
        {
            if (ShopItem.PurchaseType == PurchaseType.Weekly && ShopItem.LimitCount == 3)
            {
                if (PreType == ItemType.None)
                {
                    ShopItem.ItemType = (ItemType)(UnityEngine.Random.Range(0, 15) + 10);
                    PreType = ShopItem.ItemType;
                }
                else
                {
                    ShopItem.ItemType = (ItemType)(UnityEngine.Random.Range(0, 15) + 10);
                    while (ShopItem.ItemType == PreType)
                    {
                        ShopItem.ItemType = (ItemType)(UnityEngine.Random.Range(0, 15) + 10);
                    }
                    PreType = ShopItem.ItemType;
                }

                switch (ShopItem.ItemType)
                {
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        ShopItem.ItemCount = 5;
                        break;
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        ShopItem.ItemCount = 3;
                        break;
                    case ItemType.Core_Boar:
                    case ItemType.Core_Badger:
                    case ItemType.Core_Owl:
                    case ItemType.Core_Wolf:
                    case ItemType.Core_Fox:
                        ShopItem.ItemCount = 1;
                        break;
                }
                ShopItem.ShopTitle = ItemDataList[(int)ShopItem.ItemType].ItemName;
                ShopItem.PopupDesc = ItemDataList[(int)ShopItem.ItemType].ItemDesc;
                ShopItem.ShopDesc = ShopItem.ItemCount.ToString() + "개";
                ShopItem.ItemImg = ItemDataList[(int)ShopItem.ItemType].ItemImg;
                ShopItem.ItemImgTint = ItemDataList[(int)ShopItem.ItemType].ItemImgTint;
            }
        }
    }

    // 상점의 랜덤박스1의 보상을 랜덤하게 나오게 하는 함수
    public List<ShopItemData> ReinforceRandomBox1()
    {
        RandomBoxItemList.Clear();

        Dictionary<ItemType, ShopItemData> RandomItemDic = new Dictionary<ItemType, ShopItemData>();

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
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 6);
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 11);
                        break;
                    case ItemType.CheeseLow:
                    case ItemType.WineLow:
                    case ItemType.MeatLow:
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 16);
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
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 6)));
                        RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 11)));
                        RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                        break;
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 11)));
                        break;
                    case ItemType.CheeseLow:
                    case ItemType.WineLow:
                    case ItemType.MeatLow:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 16)));
                        RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                        break;
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 16)));
                        break;
                }
                RandomItemDic[RandItemType].ItemType = RandItemType;
                RandomItemDic[RandItemType].ItemImg = ItemDataList[(int)RandItemType].ItemImg;
                RandomItemDic[RandItemType].ItemImgTint = ItemDataList[(int)RandItemType].ItemImgTint;
                RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                RandomItemDic[RandItemType].PopupDesc = ItemDataList[(int)RandItemType].ItemDesc;
            }
        }

        foreach (var RandomItem in RandomItemDic)
        {
            AddItem(RandomItem.Key, RandomItem.Value.ItemCount);
            RandomBoxItemList.Add(RandomItem.Value);
        }
        RandomBoxItemList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });

        return RandomBoxItemList;
    }

    // 상점의 랜덤박스2의 보상을 랜덤하게 나오게 하는 함수
    public List<ShopItemData> ReinforceRandomBox2()
    {
        RandomBoxItemList.Clear();

        Dictionary<ItemType, ShopItemData> RandomItemDic = new Dictionary<ItemType, ShopItemData>();

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
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 6);
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 11);
                        break;
                    case ItemType.Core_Squirrel:
                    case ItemType.Core_Lizard:
                    case ItemType.Core_Toad:
                    case ItemType.Core_Pigeon:
                    case ItemType.Core_Mole:
                        RandomItemDic[RandItemType].ItemCount += UnityEngine.Random.Range(1, 16);
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
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 6)));
                        RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                        break;
                    case ItemType.CheeseMed:
                    case ItemType.WineMed:
                    case ItemType.MeatMed:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 11)));
                        RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                        break;
                    case ItemType.Core_Boar:
                    case ItemType.Core_Badger:
                    case ItemType.Core_Owl:
                    case ItemType.Core_Wolf:
                    case ItemType.Core_Fox:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 6)));
                        break;
                    case ItemType.Core_Ferret:
                    case ItemType.Core_Falcon:
                    case ItemType.Core_Skunk:
                    case ItemType.Core_Chameleon:
                    case ItemType.Core_Snake:
                        RandomItemDic.Add(RandItemType, new ShopItemData(UnityEngine.Random.Range(1, 11)));
                        break;

                }
                RandomItemDic[RandItemType].ItemType = RandItemType;
                RandomItemDic[RandItemType].ItemImg = ItemDataList[(int)RandItemType].ItemImg;
                RandomItemDic[RandItemType].ItemImgTint = ItemDataList[(int)RandItemType].ItemImgTint;
                RandomItemDic[RandItemType].ShopTitle = ItemDataList[(int)RandItemType].ItemName;
                RandomItemDic[RandItemType].PopupDesc = ItemDataList[(int)RandItemType].ItemDesc;
            }
        }

        foreach (var RandomItem in RandomItemDic)
        {
            AddItem(RandomItem.Key, RandomItem.Value.ItemCount);
            RandomBoxItemList.Add(RandomItem.Value);
        }
        RandomBoxItemList.Sort(delegate (ShopItemData A, ShopItemData B) { return A.ItemType.CompareTo(B.ItemType); });

        return RandomBoxItemList;
    }

    // 상점의 랜덤박스의 보상을 가져오는 함수
    public List<ShopItemData> GetRandomBoxItemList()
    {
        return RandomBoxItemList;
    }

    // 닉네임변경 함수
    public void ChangeNickName(string NewName)
    {
        SceneStarter.Instance.userElements.UserData.UserName = NewName;
    }

    // 부스터 버프 설정
    public void SetBoosterBuff(ShopType shopType, int ShopItemIndex)
    {
        ShopItemData BuyItemData = ShopItemDic[shopType][ShopItemIndex];

        // 이전 부스터가 없을때
        if (!UserData.bIsBooster)
        {
            UserData.bIsBooster = true;
            FirstBoosterTime = DateTime.Now;
            RestBoosterTime = TimeSpan.FromHours(BuyItemData.ItemCount);
        }
        // 이전 부스터가 적용중일때
        else
        {
            RestBoosterTime += TimeSpan.FromHours(BuyItemData.ItemCount);
        }
    }

    // 랜덤 강화상자1 보상목록 가져오는 함수
    public List<ShopItemData> GetRandomBox1RewardList()
    {
        return RandomBox1RewardList;
    }

    // 랜덤 강화상자2 보상목록 가져오는 함수
    public List<ShopItemData> GetRandomBox2RewardList()
    {
        return RandomBox2RewardList;
    }

    public void AddExpAndGold(int Exp, int Gold)
    {
        UserData.UserGold += Gold;
        UserData.UserCurExp += Exp;

        AddMissionCount(MissionType.Daily, 1, Gold);
        AddMissionCount(MissionType.Weekly, 2, Gold);
        AddMissionCount(MissionType.Weekly, 3, Gold);

        while (UserData.UserCurExp >= UserData.UserMaxExp)
        {
            ++UserData.UserLevel;
            UserData.UserMaxStamina += 5;
            UserData.UserCurStamina += UserData.UserMaxStamina;
            UserData.UserCurExp -= UserData.UserMaxExp;
            UserData.UserMaxExp += 100;

            bLevelUp = true;
        }
    }

    public void SetBoosterExpAndGold(int Exp, int Gold, float Percent)
    {
        UserData.UserGold += (int)(Gold * (1 + Percent));
        UserData.UserCurExp += (int)(Exp * (1 + Percent));

        AddMissionCount(MissionType.Daily, 1, (int)(Gold * (1 + Percent)));
        AddMissionCount(MissionType.Weekly, 2, (int)(Gold * (1 + Percent)));
        AddMissionCount(MissionType.Weekly, 3, (int)(Gold * (1 + Percent)));

        while (UserData.UserCurExp >= UserData.UserMaxExp)
        {
            ++UserData.UserLevel;
            UserData.UserMaxStamina += 5;
            UserData.UserCurStamina += UserData.UserMaxStamina;
            UserData.UserCurExp -= UserData.UserMaxExp;
            UserData.UserMaxExp += 100;

            bLevelUp = true;
        }
    }

    // 부스터 남은 시간
    public TimeSpan GetRestTime()
    {
        return RestBoosterTime;
    }

    public int UnitReinforceMent(CommonType UnitType)
    {
        return UnitDataList[(int)UnitType].Level;
    }

    public bool GetCurChapter(int CurChapterIndex)
    {
        return UserData.Chapter[CurChapterIndex];
    }

    public bool GetNextChapter(int CurChapterIndex)
    {
        if (CurChapterIndex + 1 >= UserData.Chapter.Count)
            return UserData.Chapter[CurChapterIndex];

        return UserData.Chapter[CurChapterIndex + 1];
    }

    public void CalculationStamina()
    {
        if (UserData.UserCurStamina < UserData.UserMaxStamina)
        {
            TimeSpan Temp = LastLoginTime - DateTime.Now;

            UserData.UserCurStamina += (int)Temp.TotalSeconds / (int)Global.StaminaChargeTime;
            if (UserData.UserCurStamina >= UserData.UserMaxStamina)
            {
                UserData.UserCurStamina = UserData.UserMaxStamina;
                ChargeStaminaTime = TimeSpan.FromSeconds(Global.StaminaChargeTime);
            }
            else if (Temp.TotalSeconds % Global.StaminaChargeTime >= ChargeStaminaTime.TotalSeconds)
            {
                ChargeStaminaTime = TimeSpan.FromSeconds(Global.StaminaChargeTime - (Temp.TotalSeconds % Global.StaminaChargeTime - ChargeStaminaTime.TotalSeconds));
                UserData.UserCurStamina++;
            }
            else
                ChargeStaminaTime = TimeSpan.FromSeconds(ChargeStaminaTime.TotalSeconds - Temp.TotalSeconds % Global.StaminaChargeTime);
        }
    }

    public void CheckReset()
    {
        if (LastLoginTime.Day != DateTime.Now.Day || LastLoginTime.Month != DateTime.Now.Month || LastLoginTime.Year != DateTime.Now.Year)
        {
            SetDailyMissions();
            ResetShopItemList(ShopType.Goods, PurchaseType.Daily);
            ResetShopItemList(ShopType.BuffGoods, PurchaseType.Daily);
            ResetShopItemList(ShopType.Growth, PurchaseType.Daily);

            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                SetWeeklyMissions();
                ResetShopItemList(ShopType.Goods, PurchaseType.Weekly);
                ResetShopItemList(ShopType.BuffGoods, PurchaseType.Weekly);
                ResetShopItemList(ShopType.Growth, PurchaseType.Weekly);
            }
        }
    }
}
