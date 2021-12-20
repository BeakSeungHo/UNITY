using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReinforceData
{
    public CommonType UnitType;
    public int UnitPerBuliding;
    public int Cost;
    public int Sight;
    public int Range;
    public int BuildCost;
    public float Damage;
    public float MaxHp;
    public float AttackSpeed;
    public float ProjectileSpeed;
    public float MoveSpeed;
    public float GenTime;

    public int Gold;
    public int CoreCount;
    public int NoneCount;
    public int CheeseLowCount, CheeseMedCount, CheeseHighCount;           
    public int WineLowCount, WineMedCount, WineHighCount;
    public int MeatLowCount, MeatMedCount, MeatHighCount;

    public ReinforceData()
    {

    }

    public ReinforceData(CommonType unitType, int unitPerBuliding, int cost, int sight, int range, int buildCost, float damage, 
                        float maxHp, float attackSpeed, float projectileSpeed, float moveSpeed, float genTime)
    {
        UnitType            = unitType;
        UnitPerBuliding     = unitPerBuliding;
        Cost                = cost;
        Sight               = sight;
        Range               = range;
        BuildCost           = buildCost;
        Damage              = damage;
        MaxHp               = maxHp;
        AttackSpeed         = attackSpeed;
        ProjectileSpeed     = projectileSpeed;
        MoveSpeed           = moveSpeed;
        GenTime             = genTime;
        Gold                = 0;
        CoreCount           = 0;
        NoneCount           = 0;
        CheeseLowCount      = 0;
        CheeseMedCount      = 0;
        CheeseHighCount     = 0;
        WineLowCount        = 0;
        WineMedCount        = 0;
        WineHighCount       = 0;
        MeatLowCount        = 0;
        MeatMedCount        = 0;
        MeatHighCount       = 0;
    }

    public ReinforceData(ReinforceData rhs)
    {
        UnitType            = rhs.UnitType;
        UnitPerBuliding     = rhs.UnitPerBuliding;
        Cost                = rhs.Cost;
        Sight               = rhs.Sight;
        Range               = rhs.Range;
        BuildCost           = rhs.BuildCost;
        Damage              = rhs.Damage;
        MaxHp               = rhs.MaxHp;
        AttackSpeed         = rhs.AttackSpeed;
        ProjectileSpeed     = rhs.ProjectileSpeed;
        MoveSpeed           = rhs.MoveSpeed;
        GenTime             = rhs.GenTime;
        Gold                = rhs.Gold;
        CoreCount           = rhs.CoreCount;
        NoneCount           = rhs.NoneCount;
        CheeseLowCount      = rhs.CheeseLowCount;
        CheeseMedCount      = rhs.CheeseMedCount;
        CheeseHighCount     = rhs.CheeseHighCount;
        WineLowCount        = rhs.WineLowCount;
        WineMedCount        = rhs.WineMedCount;
        WineHighCount       = rhs.WineHighCount;
        MeatLowCount        = rhs.MeatLowCount;
        MeatMedCount        = rhs.MeatMedCount;
        MeatHighCount       = rhs.MeatHighCount;
    }

    public static ReinforceData operator +(ReinforceData Data1, ReinforceData Data2)
    {
        ReinforceData NewData = new ReinforceData();

        NewData.UnitType        = Data1.UnitType;
        NewData.UnitPerBuliding = Data1.UnitPerBuliding + Data2.UnitPerBuliding;
        NewData.Cost            = Data1.Cost + Data2.Cost;
        NewData.Sight           = Data1.Sight + Data2.Sight;
        NewData.Range           = Data1.Range + Data2.Range;
        NewData.BuildCost       = Data1.BuildCost + Data2.BuildCost;
        NewData.Damage          = Data1.Damage + Data2.Damage;
        NewData.MaxHp           = Data1.MaxHp + Data2.MaxHp;
        NewData.AttackSpeed     = Data1.AttackSpeed + Data2.AttackSpeed;
        NewData.ProjectileSpeed = Data1.ProjectileSpeed + Data2.ProjectileSpeed;
        NewData.MoveSpeed       = Data1.MoveSpeed + Data2.MoveSpeed;
        NewData.GenTime         = Data1.GenTime + Data2.GenTime;
        NewData.Gold            = Data1.Gold + Data2.Gold;
        NewData.CoreCount       = Data1.CoreCount + Data2.CoreCount;
        NewData.CheeseLowCount  = Data1.CheeseLowCount + Data2.CheeseLowCount;
        NewData.CheeseMedCount  = Data1.CheeseMedCount + Data2.CheeseMedCount;
        NewData.CheeseHighCount = Data1.CheeseHighCount + Data2.CheeseHighCount;
        NewData.WineLowCount    = Data1.WineLowCount + Data2.WineLowCount;
        NewData.WineMedCount    = Data1.WineMedCount + Data2.WineMedCount;
        NewData.WineHighCount   = Data1.WineHighCount + Data2.WineHighCount;
        NewData.MeatLowCount    = Data1.MeatLowCount + Data2.MeatLowCount;
        NewData.MeatMedCount    = Data1.MeatMedCount + Data2.MeatMedCount;
        NewData.MeatHighCount   = Data1.MeatHighCount + Data2.MeatHighCount;

        return NewData;
    }
}

public class ReinforceElements : ScriptableObject
{
    public CommonElements commonElements = null;

    //각 레벨에 따른 각 타입별 강화수치
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelZeroData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelOneData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelTwoData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelThreeData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelFourData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelFiveData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelSixData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelSevenData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelEightData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelNineData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelTenData = null;

    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelZeroAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelOneAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelTwoAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelThreeAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelFourAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelFiveAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelSixAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelSevenAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelEightAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelNineAccData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> LevelTenAccData = null;

    //각 레벨에 따르는 각 타입별 강화수치와 기본수치 합을 저장한 데이터리스트
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusZeroData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusOneData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusTwoData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusThreeData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusFourData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusFiveData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusSixData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusSevenData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusEightData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusNineData = null;
    [ArrayElementTitle("UnitType")]
    public List<ReinforceData> CommonPlusTenData = null;

    //강화 리스트들을 저장해두는 리스트
    public List<List<ReinforceData>> ReinforceDataList = new List<List<ReinforceData>>();

    //강화 리스트들을 저장해두는 리스트
    public List<List<ReinforceData>> ReinforceAccDataList = new List<List<ReinforceData>>();

    //강화수치와 기본수치 합을 저장해두는 리스트
    public List<List<ReinforceData>> CommonPlusDataList = new List<List<ReinforceData>>();

    public void Refresh()
    {
        LevelZeroData.Clear();
        LevelOneData.Clear();
        LevelTwoData.Clear();
        LevelThreeData.Clear();
        LevelFourData.Clear();
        LevelFiveData.Clear();
        LevelSixData.Clear();
        LevelSevenData.Clear();
        LevelEightData.Clear();
        LevelNineData.Clear();
        LevelTenData.Clear();

        string[] ReinforceDataArr = ExMethods.CSVReader("Data/CSV/ReinforceData");

        for (int i = 1; i < ReinforceDataArr.Length; ++i)
        {
            string[] ReinforceData = ReinforceDataArr[i].Split(',');

            ReinforceData InsertData = new ReinforceData();

            if (ReinforceData[0].ToEnum<CommonType>() == -1)
                continue;

            InsertData.UnitType = (CommonType)ReinforceData[0].ToEnum<CommonType>();
            InsertData.UnitPerBuliding = int.Parse(ReinforceData[2]);
            InsertData.Cost = int.Parse(ReinforceData[3]);
            InsertData.Sight = int.Parse(ReinforceData[4]);
            InsertData.Range = int.Parse(ReinforceData[5]);
            InsertData.BuildCost = int.Parse(ReinforceData[6]);
            InsertData.Damage = float.Parse(ReinforceData[7]);
            InsertData.MaxHp = float.Parse(ReinforceData[8]);
            InsertData.AttackSpeed = float.Parse(ReinforceData[9]);
            InsertData.ProjectileSpeed = float.Parse(ReinforceData[10]);
            InsertData.MoveSpeed = float.Parse(ReinforceData[11]);
            InsertData.GenTime = float.Parse(ReinforceData[12]);
            InsertData.Gold = int.Parse(ReinforceData[13]);
            InsertData.CoreCount = int.Parse(ReinforceData[14]);
            InsertData.NoneCount = int.Parse(ReinforceData[15]);
            InsertData.CheeseLowCount = int.Parse(ReinforceData[16]);
            InsertData.CheeseMedCount = int.Parse(ReinforceData[17]);
            InsertData.CheeseHighCount = int.Parse(ReinforceData[18]);
            InsertData.WineLowCount = int.Parse(ReinforceData[19]);
            InsertData.WineMedCount = int.Parse(ReinforceData[20]);
            InsertData.WineHighCount = int.Parse(ReinforceData[21]);
            InsertData.MeatLowCount = int.Parse(ReinforceData[22]);
            InsertData.MeatMedCount = int.Parse(ReinforceData[23]);
            InsertData.MeatHighCount = int.Parse(ReinforceData[24]);
            
            switch (int.Parse(ReinforceData[1]))
            {
                case 0:
                    LevelZeroData.Add(InsertData);
                    break;
                case 1:
                    LevelOneData.Add(InsertData);
                    break;
                case 2:
                    LevelTwoData.Add(InsertData);
                    break;
                case 3:
                    LevelThreeData.Add(InsertData);
                    break;
                case 4:
                    LevelFourData.Add(InsertData);
                    break;
                case 5:
                    LevelFiveData.Add(InsertData);
                    break;
                case 6:
                    LevelSixData.Add(InsertData);
                    break;
                case 7:
                    LevelSevenData.Add(InsertData);
                    break;
                case 8:
                    LevelEightData.Add(InsertData);
                    break;
                case 9:
                    LevelNineData.Add(InsertData);
                    break;
                case 10:
                    LevelTenData.Add(InsertData);
                    break;
            }
        }

        LevelZeroAccData.Clear();
        LevelOneAccData.Clear();
        LevelTwoAccData.Clear();
        LevelThreeAccData.Clear();
        LevelFourAccData.Clear();
        LevelFiveAccData.Clear();
        LevelSixAccData.Clear();
        LevelSevenAccData.Clear();
        LevelEightAccData.Clear();
        LevelNineAccData.Clear();
        LevelTenAccData.Clear();

        for(int i = 0; i < 15; ++i)
        {
            ReinforceData InsertData = new ReinforceData();

            InsertData = LevelZeroData[i];
            LevelZeroAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i];
            LevelOneAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i];
            LevelTwoAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i];
            LevelThreeAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i];
            LevelFourAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i] + LevelFiveData[i];
            LevelFiveAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i] + LevelFiveData[i]
                                                + LevelSixData[i];
            LevelSixAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i] + LevelFiveData[i]
                                                + LevelSixData[i] + LevelSevenData[i];
            LevelSevenAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i] + LevelFiveData[i]
                                                + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i];
            LevelEightAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i]+ LevelFiveData[i]          
                                                + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i] + LevelNineData[i];
            LevelNineAccData.Add(new ReinforceData(InsertData));
            InsertData = LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + LevelFourData[i]+ LevelFiveData[i] 
                                                + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i] + LevelNineData[i] + LevelTenData[i];
            LevelTenAccData.Add(new ReinforceData(InsertData));
        }

        CommonPlusZeroData.Clear();
        CommonPlusOneData.Clear();
        CommonPlusTwoData.Clear();
        CommonPlusThreeData.Clear();
        CommonPlusFourData.Clear();
        CommonPlusFiveData.Clear();
        CommonPlusSixData.Clear();
        CommonPlusSevenData.Clear();
        CommonPlusEightData.Clear();
        CommonPlusNineData.Clear();
        CommonPlusTenData.Clear();

        for (int i = 0; i < 15; ++i)
        {
            ReinforceData InsertCommonData = new ReinforceData();

            InsertCommonData.UnitType = commonElements.CommonDataList[i].CommonType;
            InsertCommonData.UnitPerBuliding = commonElements.CommonDataList[i].UnitPerBuliding;
            InsertCommonData.Cost = commonElements.CommonDataList[i].Cost;
            InsertCommonData.Sight = commonElements.CommonDataList[i].Sight;
            InsertCommonData.Range = commonElements.CommonDataList[i].Range;
            InsertCommonData.BuildCost = commonElements.CommonDataList[i].BuildCost;
            InsertCommonData.Damage = commonElements.CommonDataList[i].Damage;
            InsertCommonData.MaxHp = commonElements.CommonDataList[i].MaxHp;
            InsertCommonData.AttackSpeed = commonElements.CommonDataList[i].AttackSpeed;
            InsertCommonData.ProjectileSpeed = commonElements.CommonDataList[i].ProjectileSpeed;
            InsertCommonData.MoveSpeed = commonElements.CommonDataList[i].MoveSpeed;
            InsertCommonData.GenTime = commonElements.CommonDataList[i].GenTime;
            InsertCommonData.Gold = 0;
            InsertCommonData.CoreCount = 0;
            InsertCommonData.NoneCount = 0;
            InsertCommonData.CheeseLowCount = 0;
            InsertCommonData.CheeseMedCount = 0;
            InsertCommonData.CheeseHighCount = 0;
            InsertCommonData.WineLowCount = 0;
            InsertCommonData.WineMedCount = 0;
            InsertCommonData.WineHighCount = 0;
            InsertCommonData.MeatLowCount = 0;
            InsertCommonData.MeatMedCount = 0;
            InsertCommonData.MeatHighCount = 0;

            CommonPlusZeroData.Add(new ReinforceData(InsertCommonData));
            CommonPlusOneData.Add(new ReinforceData(InsertCommonData));
            CommonPlusTwoData.Add(new ReinforceData(InsertCommonData));
            CommonPlusThreeData.Add(new ReinforceData(InsertCommonData));
            CommonPlusFourData.Add(new ReinforceData(InsertCommonData));
            CommonPlusFiveData.Add(new ReinforceData(InsertCommonData));
            CommonPlusSixData.Add(new ReinforceData(InsertCommonData));
            CommonPlusSevenData.Add(new ReinforceData(InsertCommonData));
            CommonPlusEightData.Add(new ReinforceData(InsertCommonData));
            CommonPlusNineData.Add(new ReinforceData(InsertCommonData));
            CommonPlusTenData.Add(new ReinforceData(InsertCommonData));
        }

        for (int i = 0; i < 15; ++i)
        {
            CommonPlusZeroData[i] = CommonPlusZeroData[i] + LevelZeroData[i];
            CommonPlusOneData[i] = CommonPlusOneData[i] + LevelZeroData[i] + LevelOneData[i];
            CommonPlusTwoData[i] = CommonPlusTwoData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i];
            CommonPlusThreeData[i] = CommonPlusThreeData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i];
            CommonPlusFourData[i] = CommonPlusFourData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + 
                                        LevelFourData[i];
            CommonPlusFiveData[i] = CommonPlusFiveData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + 
                                        LevelFourData[i] + LevelFiveData[i];
            CommonPlusSixData[i] = CommonPlusSixData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + 
                                        LevelFourData[i] + LevelFiveData[i] + LevelSixData[i];
            CommonPlusSevenData[i] = CommonPlusSevenData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] 
                                        + LevelFourData[i] + LevelFiveData[i] + LevelSixData[i] + LevelSevenData[i];
            CommonPlusEightData[i] = CommonPlusEightData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] +
                                        LevelFourData[i] + LevelFiveData[i] + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i];
            CommonPlusNineData[i] = CommonPlusNineData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + 
                                        LevelFourData[i] + LevelFiveData[i] + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i] + LevelNineData[i];
            CommonPlusTenData[i] = CommonPlusTenData[i] + LevelZeroData[i] + LevelOneData[i] + LevelTwoData[i] + LevelThreeData[i] + 
                                        LevelFourData[i] + LevelFiveData[i] + LevelSixData[i] + LevelSevenData[i] + LevelEightData[i] + LevelNineData[i] + LevelTenData[i];
        }
    }

    public void InitializeElement()
    {
        ReinforceDataList.Add(LevelZeroData);
        ReinforceDataList.Add(LevelOneData);
        ReinforceDataList.Add(LevelTwoData);
        ReinforceDataList.Add(LevelThreeData);
        ReinforceDataList.Add(LevelFourData);
        ReinforceDataList.Add(LevelFiveData);
        ReinforceDataList.Add(LevelSixData);
        ReinforceDataList.Add(LevelSevenData);
        ReinforceDataList.Add(LevelEightData);
        ReinforceDataList.Add(LevelNineData);
        ReinforceDataList.Add(LevelTenData);
        
        ReinforceAccDataList.Add(LevelZeroAccData);
        ReinforceAccDataList.Add(LevelOneAccData);
        ReinforceAccDataList.Add(LevelTwoAccData);
        ReinforceAccDataList.Add(LevelThreeAccData);
        ReinforceAccDataList.Add(LevelFourAccData);
        ReinforceAccDataList.Add(LevelFiveAccData);
        ReinforceAccDataList.Add(LevelSixAccData);
        ReinforceAccDataList.Add(LevelSevenAccData);
        ReinforceAccDataList.Add(LevelEightAccData);
        ReinforceAccDataList.Add(LevelNineAccData);
        ReinforceAccDataList.Add(LevelTenAccData);

        CommonPlusDataList.Add(CommonPlusZeroData);
        CommonPlusDataList.Add(CommonPlusOneData);
        CommonPlusDataList.Add(CommonPlusTwoData);
        CommonPlusDataList.Add(CommonPlusThreeData);
        CommonPlusDataList.Add(CommonPlusFiveData);
        CommonPlusDataList.Add(CommonPlusFourData);
        CommonPlusDataList.Add(CommonPlusSixData);
        CommonPlusDataList.Add(CommonPlusSevenData);
        CommonPlusDataList.Add(CommonPlusEightData);
        CommonPlusDataList.Add(CommonPlusNineData);
        CommonPlusDataList.Add(CommonPlusTenData);

        for (int i = 0; i < CommonPlusDataList.Count; ++i)
        {
            for (int j = 0; j < ReinforceDataList[i].Count; ++j)
            {
                int PlusUnitPerBuliding = 0;
                int PlusCost = 0;
                int PlusSight = 0;
                int PlusRange = 0;
                int PlusBuildCost = 0;
                float PlusMaxHp = 0;
                float PlusAttackSpeed = 0;
                float PlusProjectileSpeed = 0;
                float PlusMoveSpeed = 0;
                float PlusGenTime = 0;

                for (int k = 0; k <= i; ++k)
                {
                    PlusUnitPerBuliding += ReinforceDataList[k][j].UnitPerBuliding;
                    PlusCost += ReinforceDataList[k][j].Cost;
                    PlusSight += ReinforceDataList[k][j].Sight;
                    PlusRange += ReinforceDataList[k][j].Range;
                    PlusBuildCost += ReinforceDataList[k][j].BuildCost;
                    PlusMaxHp += ReinforceDataList[k][j].MaxHp;
                    PlusAttackSpeed += ReinforceDataList[k][j].AttackSpeed;
                    PlusProjectileSpeed += ReinforceDataList[k][j].ProjectileSpeed;
                    PlusMoveSpeed += ReinforceDataList[k][j].MoveSpeed;
                    PlusGenTime += ReinforceDataList[k][j].GenTime;
                }

                CommonPlusDataList[i][j].UnitPerBuliding = SceneStarter.Instance.commonElements.CommonDataList[j].UnitPerBuliding + PlusUnitPerBuliding;
                CommonPlusDataList[i][j].Cost = SceneStarter.Instance.commonElements.CommonDataList[j].Cost + PlusCost;
                CommonPlusDataList[i][j].Sight = SceneStarter.Instance.commonElements.CommonDataList[j].Sight + PlusSight;
                CommonPlusDataList[i][j].Range = SceneStarter.Instance.commonElements.CommonDataList[j].Range + PlusRange;
                CommonPlusDataList[i][j].BuildCost = SceneStarter.Instance.commonElements.CommonDataList[j].BuildCost + PlusBuildCost;
                CommonPlusDataList[i][j].MaxHp = SceneStarter.Instance.commonElements.CommonDataList[j].MaxHp + PlusMaxHp;
                CommonPlusDataList[i][j].AttackSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].AttackSpeed + PlusAttackSpeed;
                CommonPlusDataList[i][j].ProjectileSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].ProjectileSpeed + PlusProjectileSpeed;
                CommonPlusDataList[i][j].MoveSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].MoveSpeed + PlusMoveSpeed;
                CommonPlusDataList[i][j].GenTime = SceneStarter.Instance.commonElements.CommonDataList[j].GenTime + PlusGenTime;
            }
        }
    }
    public void InitializeElement_Reinforce()
    {
        ReinforceDataList.Add(LevelZeroData);
        ReinforceDataList.Add(LevelOneData);
        ReinforceDataList.Add(LevelTwoData);
        ReinforceDataList.Add(LevelThreeData);
        ReinforceDataList.Add(LevelFourData);
        ReinforceDataList.Add(LevelFiveData);
        ReinforceDataList.Add(LevelSixData);
        ReinforceDataList.Add(LevelSevenData);
        ReinforceDataList.Add(LevelEightData);
        ReinforceDataList.Add(LevelNineData);
        ReinforceDataList.Add(LevelTenData);
    }
    public void InitializeElement_CommonPlus()
    {
        CommonPlusDataList.Add(CommonPlusZeroData);
        CommonPlusDataList.Add(CommonPlusOneData);
        CommonPlusDataList.Add(CommonPlusTwoData);
        CommonPlusDataList.Add(CommonPlusThreeData);
        CommonPlusDataList.Add(CommonPlusFiveData);
        CommonPlusDataList.Add(CommonPlusFourData);
        CommonPlusDataList.Add(CommonPlusSixData);
        CommonPlusDataList.Add(CommonPlusSevenData);
        CommonPlusDataList.Add(CommonPlusEightData);
        CommonPlusDataList.Add(CommonPlusNineData);
        CommonPlusDataList.Add(CommonPlusTenData);
    }
    public void InitializeElement_InputData()
    {
        for (int i = 0; i < CommonPlusDataList.Count; ++i)
        {
            for (int j = 0; j < ReinforceDataList[i].Count; ++j)
            {
                int PlusUnitPerBuliding = 0;
                int PlusCost = 0;
                int PlusSight = 0;
                int PlusRange = 0;
                int PlusBuildCost = 0;
                float PlusMaxHp = 0;
                float PlusAttackSpeed = 0;
                float PlusProjectileSpeed = 0;
                float PlusMoveSpeed = 0;
                float PlusGenTime = 0;

                for (int k = 0; k <= i; ++k)
                {
                    PlusUnitPerBuliding += ReinforceDataList[k][j].UnitPerBuliding;
                    PlusCost += ReinforceDataList[k][j].Cost;
                    PlusSight += ReinforceDataList[k][j].Sight;
                    PlusRange += ReinforceDataList[k][j].Range;
                    PlusBuildCost += ReinforceDataList[k][j].BuildCost;
                    PlusMaxHp += ReinforceDataList[k][j].MaxHp;
                    PlusAttackSpeed += ReinforceDataList[k][j].AttackSpeed;
                    PlusProjectileSpeed += ReinforceDataList[k][j].ProjectileSpeed;
                    PlusMoveSpeed += ReinforceDataList[k][j].MoveSpeed;
                    PlusGenTime += ReinforceDataList[k][j].GenTime;
                }

                CommonPlusDataList[i][j].UnitPerBuliding = SceneStarter.Instance.commonElements.CommonDataList[j].UnitPerBuliding + PlusUnitPerBuliding;
                CommonPlusDataList[i][j].Cost = SceneStarter.Instance.commonElements.CommonDataList[j].Cost + PlusCost;
                CommonPlusDataList[i][j].Sight = SceneStarter.Instance.commonElements.CommonDataList[j].Sight + PlusSight;
                CommonPlusDataList[i][j].Range = SceneStarter.Instance.commonElements.CommonDataList[j].Range + PlusRange;
                CommonPlusDataList[i][j].BuildCost = SceneStarter.Instance.commonElements.CommonDataList[j].BuildCost + PlusBuildCost;
                CommonPlusDataList[i][j].MaxHp = SceneStarter.Instance.commonElements.CommonDataList[j].MaxHp + PlusMaxHp;
                CommonPlusDataList[i][j].AttackSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].AttackSpeed + PlusAttackSpeed;
                CommonPlusDataList[i][j].ProjectileSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].ProjectileSpeed + PlusProjectileSpeed;
                CommonPlusDataList[i][j].MoveSpeed = SceneStarter.Instance.commonElements.CommonDataList[j].MoveSpeed + PlusMoveSpeed;
                CommonPlusDataList[i][j].GenTime = SceneStarter.Instance.commonElements.CommonDataList[j].GenTime + PlusGenTime;
            }
        }
    }
    // 타입과 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceCurData(CommonType UnitType, int CurLevel)
    {
        if (UnitType > CommonType.Fox)
            return ReinforceDataList[0][0];

        return ReinforceDataList[CurLevel][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceCurData(int UnitTypeIndex, int CurLevel)
    {
        if (UnitTypeIndex > 14)
            return ReinforceDataList[0][0];

        return ReinforceDataList[CurLevel][UnitTypeIndex];
    }

    // 타입과 현재 레벨을 이용하여 다음 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceNextData(CommonType UnitType, int CurLevel)
    {
        if (CurLevel >= 10)
            return ReinforceDataList[0][(int)UnitType];

        return ReinforceDataList[CurLevel+1][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceNextData(int UnitTypeIndex, int CurLevel)
    {
        if (CurLevel >= 10)
            return ReinforceDataList[0][UnitTypeIndex];

        return ReinforceDataList[CurLevel+1][UnitTypeIndex];
    }

    // 타입과 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceAccCurData(CommonType UnitType, int CurLevel)
    {
        if (UnitType > CommonType.Fox)
            return ReinforceAccDataList[0][0];

        return ReinforceAccDataList[CurLevel][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceAccCurData(int UnitTypeIndex, int CurLevel)
    {
        if (UnitTypeIndex > 14)
            return ReinforceAccDataList[0][0];

        return ReinforceAccDataList[CurLevel][UnitTypeIndex];
    }

    // 타입과 현재 레벨을 이용하여 다음 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceAccNextData(CommonType UnitType, int CurLevel)
    {
        if (CurLevel >= 10)
            return ReinforceAccDataList[0][(int)UnitType];

        return ReinforceAccDataList[CurLevel + 1][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 현재 레벨의 강화데이터를 가져옴
    public ReinforceData GetReinforceAccNextData(int UnitTypeIndex, int CurLevel)
    {
        if (CurLevel >= 10)
            return ReinforceAccDataList[0][UnitTypeIndex];

        return ReinforceAccDataList[CurLevel + 1][UnitTypeIndex];
    }

    // 타입과 현재 레벨을 이용하여 현재 레벨의 강화가 완료된 데이터를 가져옴
    public ReinforceData CompleteReinforceCurData(CommonType UnitType, int CurLevel)
    {
        return CommonPlusDataList[CurLevel][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 현재 레벨의 강화가 완료된 데이터를 가져옴
    public ReinforceData CompleteReinforceCurData(int UnitTypeIndex, int CurLevel)
    {
        return CommonPlusDataList[CurLevel][UnitTypeIndex];
    }

    // 타입과 현재 레벨을 이용하여 다음 레벨의 강화가 완료된 데이터를 가져옴
    public ReinforceData CompleteReinforceNextData(CommonType UnitType, int CurLevel)
    {
        if (CurLevel >= 10)
            return CommonPlusDataList[10][(int)UnitType];

        return CommonPlusDataList[CurLevel+1][(int)UnitType];
    }

    // 유닛인덱스(유닛타입)와 현재 레벨을 이용하여 다음 레벨의 강화가 완료된 데이터를 가져옴
    public ReinforceData CompleteReinforceNextData(int UnitTypeIndex, int CurLevel)
    {
        if (CurLevel >= 10)
            return CommonPlusDataList[10][UnitTypeIndex];

        return CommonPlusDataList[CurLevel+1][UnitTypeIndex];
    }

    //현재 레벨과 유닛인덱스(유닛타입)을 이용하여 다음레벨로 레벨업
    public void LevelUp(int CurLevel, int UnitTypeIndex)
    {
        //만약 최대레벨에서 레벨업을 하려고하면 False를 반환
        if (LevelUpCheck(CurLevel, UnitTypeIndex))
        {
            if (SceneStarter.Instance.userElements.LevelPlus(UnitTypeIndex))
            {
                SceneStarter.Instance.userElements.UserData.UserGold -= ReinforceDataList[CurLevel + 1][UnitTypeIndex].Gold;
                SceneStarter.Instance.userElements.UseCoreCount(UnitTypeIndex, ReinforceDataList[CurLevel + 1][UnitTypeIndex].CoreCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseLow, ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseMed, ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseHigh, ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseHighCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatLow, ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatMed, ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatHigh, ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatHighCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineLow, ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineMed, ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineHigh, ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineHighCount);
            }
        }
    }

    //현재 레벨과 유닛타입을 이용하여 다음레벨로 레벨업
    public void LevelUp(int CurLevel, CommonType UnitType)
    {
        int iType = (int)UnitType;
        //만약 최대레벨에서 레벨업을 하려고하면 False를 반환
        if (LevelUpCheck(CurLevel, UnitType))
        {
            if (SceneStarter.Instance.userElements.LevelPlus(UnitType))
            {
                SceneStarter.Instance.userElements.UserData.UserGold -= ReinforceDataList[CurLevel + 1][iType].Gold;
                SceneStarter.Instance.userElements.UseCoreCount(iType, ReinforceDataList[CurLevel + 1][iType].CoreCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseLow, ReinforceDataList[CurLevel + 1][iType].CheeseLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseMed, ReinforceDataList[CurLevel + 1][iType].CheeseMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.CheeseHigh, ReinforceDataList[CurLevel + 1][iType].CheeseHighCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatLow, ReinforceDataList[CurLevel + 1][iType].MeatLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatMed, ReinforceDataList[CurLevel + 1][iType].MeatMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.MeatHigh, ReinforceDataList[CurLevel + 1][iType].MeatHighCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineLow, ReinforceDataList[CurLevel + 1][iType].WineLowCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineMed, ReinforceDataList[CurLevel + 1][iType].WineMedCount);
                SceneStarter.Instance.userElements.UseItem(ItemType.WineHigh, ReinforceDataList[CurLevel + 1][iType].WineHighCount);
            }
        }
    }

    //유닛 인덱스와 레벨을 가져와 다음레벨로 갈수잇는지 검사
    public bool LevelUpCheck(int CurLevel, int UnitTypeIndex)
    {
        if (CurLevel >= 10)
            return false;

        if (SceneStarter.Instance.userElements.UserData.UserGold >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].Gold &&
            SceneStarter.Instance.userElements.GetCoreCount(UnitTypeIndex) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].CoreCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseLow) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseMed) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseHigh) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].CheeseHighCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatLow) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatMed) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatHigh) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].MeatHighCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineLow) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineMed) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineHigh) >= ReinforceDataList[CurLevel + 1][UnitTypeIndex].WineHighCount)
        {
            return true;
        }
            
        return false;
    }
    //유닛 타입과 레벨을 가져와 다음레벨로 갈수잇는지 검사
    public bool LevelUpCheck(int CurLevel, CommonType UnitType)
    {
        if (CurLevel >= 10)
            return false;

        int iType = (int)UnitType;
        if (SceneStarter.Instance.userElements.UserData.UserGold >= ReinforceDataList[CurLevel + 1][iType].Gold &&
            SceneStarter.Instance.userElements.GetCoreCount(iType) >= ReinforceDataList[CurLevel+1][iType].CoreCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseLow) >= ReinforceDataList[CurLevel + 1][iType].CheeseLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseMed) >= ReinforceDataList[CurLevel + 1][iType].CheeseMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.CheeseHigh) >= ReinforceDataList[CurLevel + 1][iType].CheeseHighCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatLow) >= ReinforceDataList[CurLevel + 1][iType].MeatLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatMed) >= ReinforceDataList[CurLevel + 1][iType].MeatMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.MeatHigh) >= ReinforceDataList[CurLevel + 1][iType].MeatHighCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineLow) >= ReinforceDataList[CurLevel + 1][iType].WineLowCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineMed) >= ReinforceDataList[CurLevel + 1][iType].WineMedCount &&
            SceneStarter.Instance.userElements.GetItemCount(ItemType.WineHigh) >= ReinforceDataList[CurLevel + 1][iType].WineHighCount)
        {
            return true;
        }

        return false;
    }

}
