using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum CommonType
{
    Error = -1,                                     // 에러
    Squirrel, Lizard, Toad, Pigeon, Mole,           // 1티어
    Ferret, Falcon, Skunk, Chameleon, Snake,        // 2티어
    Boar, Badger, Owl, Wolf, Fox,                   // 3티어
    Wire, Mine, Turret, Balloon, Cannon,            // 방어건물
    WarrenT1, WarrenT2, WarrenT3, Gristmill, Farm,  // 생산건물
    Mouse, Pig,                                     // 특수유닛
    Cabin, CampFire,                                // 중립
    Commander,                                      // 커맨더
    MoleeMerge,                                     // 두더지 건물
    End
}

public class CommonTypeComparer : IEqualityComparer<CommonType>
{
    bool IEqualityComparer<CommonType>.Equals(CommonType x, CommonType y)
    {
        return x == y;
    }

    int IEqualityComparer<CommonType>.GetHashCode(CommonType obj)
    {
        return (int)obj;
    }
}

public enum PlaceType { Ground, Air, End }

public enum AttackType { Ground, Air, GroundAndAir, End }

[System.Serializable]
public class CommonData
{
    public CommonType CommonType;
    public PlaceType PlaceType;
    public AttackType AttackType;
    public Pool_ObjType PoolType;

    public int UnitPerBuliding;
    public int Sight;
    public int Range;
    public int Cost;

    public float Damage;
    public float MaxHp;
    public float AttackSpeed;
    public float ProjectileSpeed;
    public float MoveSpeed;
    public float GenTime;

    public string Name;
    public string Animal;
    public string Sentence;
    public int BuildCost;

    public CommonData()
    {
    }

    public CommonData(int Type)
    {
        CommonType = (CommonType)Type;
    }

    public CommonData(CommonData rhs)
    {
        CommonType = rhs.CommonType;
        PlaceType = rhs.PlaceType;
        AttackType = rhs.AttackType;

        UnitPerBuliding = rhs.UnitPerBuliding;
        Sight = rhs.Sight;
        Range = rhs.Range;
        Cost = rhs.Cost;

        Damage = rhs.Damage;
        MaxHp = rhs.MaxHp;
        ProjectileSpeed = rhs.ProjectileSpeed;
        AttackSpeed = rhs.AttackSpeed;
        MoveSpeed = rhs.MoveSpeed;
        GenTime = rhs.GenTime;

        Name = rhs.Name;
        Sentence = rhs.Sentence;
    }
}

public class CommonElements : ScriptableObject
{
    [ArrayElementTitle("CommonType")]
    [SerializeField]
    public List<CommonData> CommonDataList;
    
    public void Refresh()
    {
        CommonDataList.Clear();

        string[] CommonDataArr = ExMethods.CSVReader("Data/CSV/CommonData");

        for(int i = 1; i < CommonDataArr.Length; ++i)
        {
            string[] CommonData = CommonDataArr[i].Split(',');

            CommonData InsertData = new CommonData();

            InsertData.CommonType = (CommonType)CommonData[0].ToEnum<CommonType>();
            InsertData.PlaceType = (PlaceType)CommonData[1].ToEnum<PlaceType>();
            InsertData.AttackType = (AttackType)CommonData[2].ToEnum<AttackType>();
            InsertData.PoolType = (Pool_ObjType)CommonData[3].ToEnum<Pool_ObjType>();
            InsertData.UnitPerBuliding = int.Parse(CommonData[4]);
            InsertData.Sight = int.Parse(CommonData[5]);
            InsertData.Range = int.Parse(CommonData[6]);
            InsertData.Cost = int.Parse(CommonData[7]);
            InsertData.Damage = float.Parse(CommonData[8]);
            InsertData.MaxHp = float.Parse(CommonData[9]);
            InsertData.AttackSpeed = float.Parse(CommonData[10]);
            InsertData.ProjectileSpeed = float.Parse(CommonData[11]);
            InsertData.MoveSpeed = float.Parse(CommonData[12]);
            InsertData.GenTime = float.Parse(CommonData[13]);
            InsertData.Name = CommonData[14];
            InsertData.Animal = CommonData[15];
            InsertData.Sentence = CommonData[16];
            InsertData.BuildCost = int.Parse(CommonData[17]);

            CommonDataList.Add(InsertData);
        }
    }

}
