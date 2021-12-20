using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-31
///  
///  Desc.
///     애니메이터를 가져와서 관리하는 Elements 스크립트
/// 
/// 
/// </summary>

public enum AniType
{
    Error = -1, Idle,Run,RallyRun,RallyStand,AttackIdle,Cast,Death,CastRun,SwimAttackIdle,SwimCast,SwimIdle,SwimRun,SwimCastRun
}

public class AnimatorElements : ScriptableObject
{
    // 애니메이터를 가져와서 저장하고 다른곳에서 사용할 딕셔너리 공간
    public Dictionary<Camp, RuntimeAnimatorController> ComAniDic = new Dictionary<Camp, RuntimeAnimatorController>();
    public Dictionary<CommonType, RuntimeAnimatorController> UnitAniDic = new Dictionary<CommonType, RuntimeAnimatorController>();
    public Dictionary<CommonType, RuntimeAnimatorController> BuildAniDic = new Dictionary<CommonType, RuntimeAnimatorController>();

    // 애니메이션 저장공간
    public Dictionary<Camp, Dictionary<AniType, AnimationClip>> ComAnimationDic = new Dictionary<Camp, Dictionary<AniType, AnimationClip>>();
    public Dictionary<CommonType, Dictionary<AniType, AnimationClip>> UnitAnimationDic = new Dictionary<CommonType, Dictionary<AniType, AnimationClip>>();
    
    // 초기화함수
    public void InitializeElement()
    {
        //Resources폴더에 있는 애니메이터를 가져와서 배열에 저장
        RuntimeAnimatorController[] ComAni = Resources.LoadAll<RuntimeAnimatorController>("Animations/Commander");
        RuntimeAnimatorController[] UnitAni = Resources.LoadAll<RuntimeAnimatorController>("Animations/Unit");
        RuntimeAnimatorController[] BuildAni = Resources.LoadAll<RuntimeAnimatorController>("Animations/Buildings");
        
        //가져온 애니메이터들을 각 딕셔너리에 추가
        foreach (var i in ComAni)
        {
            //ToEnum은 string을 <CommonType>으로 변경해주는 코드(ExMethods.cs에 있음)
            //실패시 Animator Failed 출력 그 외는 딕셔너리에 추가
            if (i.name.ToEnum<Camp>().Equals((int)Camp.Error))
                continue;
            else if (!ComAniDic.ContainsKey((Camp)i.name.ToEnum<Camp>()))
                ComAniDic.Add((Camp)i.name.ToEnum<Camp>(), i);
        }

        foreach (var i in UnitAni)
        {
            if (i.name.ToEnum<CommonType>().Equals((int)CommonType.Error))
                continue;
            else if (!UnitAniDic.ContainsKey((CommonType)i.name.ToEnum<CommonType>()))
                UnitAniDic.Add((CommonType)i.name.ToEnum<CommonType>(), i);
        }

        foreach (var i in BuildAni)
        {
            if (i.name.ToEnum<CommonType>().Equals((int)CommonType.Error))
                continue;
            else if (!BuildAniDic.ContainsKey((CommonType)i.name.ToEnum<CommonType>()))
                BuildAniDic.Add((CommonType)i.name.ToEnum<CommonType>(), i);
        }
        
        //Unit 폴더의 애니메이션 가져오기
        string[] Temp = Directory.GetDirectories(Application.dataPath + "/Resources/Animations/Unit");
        
        for(int i = 0; i < Temp.Length; ++i)
        {
            string[] Path = Directory.GetDirectories(Temp[i]);
            string subPath = Temp[i].Substring(Temp[i].LastIndexOf("\\")+1, Temp[i].Length - Temp[i].LastIndexOf("\\")-1);
            
            for(int j = 0; j < Path.Length; ++j)
            {
                string Name = Path[j].Substring(Path[j].LastIndexOf("\\")+1, Path[j].Length - Path[j].LastIndexOf("\\")-1);

                if (Name.ToEnum<CommonType>().Equals((int)CommonType.Error))
                    continue;
                else if(!UnitAnimationDic.ContainsKey((CommonType)Name.ToEnum<CommonType>()))
                {
                    AnimationClip[] Ani = Resources.LoadAll<AnimationClip>("Animations/Unit/" + subPath + "/" + Name);
                    Dictionary<AniType, AnimationClip> TempUnitAniDic = new Dictionary<AniType, AnimationClip>();

                    for (int k = 0; k < Ani.Length; ++k)
                    {
                        string AnimationType = Ani[k].name.Substring(Ani[k].name.LastIndexOf("_")+1, Ani[k].name.Length - Ani[k].name.LastIndexOf("_")-1);
                        

                        if (AnimationType.ToEnum<AniType>().Equals((int)AniType.Error))
                            continue;
                        else
                            TempUnitAniDic.Add((AniType)AnimationType.ToEnum<AniType>(), Ani[k]);
                    }
                    if (TempUnitAniDic.Count > 0)
                        UnitAnimationDic.Add((CommonType)Name.ToEnum<CommonType>(), TempUnitAniDic);
                }
                   

            }
        }

        Temp = Directory.GetDirectories(Application.dataPath + "/Resources/Animations/Commander");

        for (int i = 0; i < Temp.Length; ++i)
        {
            string Name = Temp[i].Substring(Temp[i].LastIndexOf("\\") + 1, Temp[i].Length - Temp[i].LastIndexOf("\\") - 1);

            if (Name.ToEnum<Camp>().Equals((int)Camp.Error))
                continue;
            else if (!ComAnimationDic.ContainsKey((Camp)Name.ToEnum<Camp>()))
            {
                AnimationClip[] Ani = Resources.LoadAll<AnimationClip>("Animations/Commander/" + Name);
                Dictionary<AniType, AnimationClip> TempUnitAniDic = new Dictionary<AniType, AnimationClip>();

                for (int j = 0; j < Ani.Length; ++j)
                {
                    string AnimationType = Ani[j].name.Substring(Ani[j].name.LastIndexOf("_") + 1, Ani[j].name.Length - Ani[j].name.LastIndexOf("_") - 1);

                    if (AnimationType.ToEnum<AniType>().Equals((int)AniType.Error))
                        continue;
                    else
                        TempUnitAniDic.Add((AniType)AnimationType.ToEnum<AniType>(), Ani[j]);
                }
                if (TempUnitAniDic.Count > 0)
                    ComAnimationDic.Add((Camp)Name.ToEnum<Camp>(), TempUnitAniDic);
            }

        }

    }

    public AnimationClip GetComAnimation(Camp Type, AniType Ani)
    {
        if (!ComAnimationDic.ContainsKey(Type) || !ComAnimationDic[Type].ContainsKey(Ani))
            return null;

        return ComAnimationDic[Type][Ani];
    }

    public AnimationClip GetComAnimation(int Index, AniType Ani)
    {
        if (!ComAnimationDic.ContainsKey((Camp)Index) || !ComAnimationDic[(Camp)Index].ContainsKey(Ani))
            return null;

        return ComAnimationDic[(Camp)Index][Ani];
    }

    public AnimationClip GetUnitAnimation(CommonType Type,AniType Ani)
    {
        if (!UnitAnimationDic.ContainsKey(Type) || !UnitAnimationDic[Type].ContainsKey(Ani))
            return null;
        
        return UnitAnimationDic[Type][Ani];
    }

    public AnimationClip GetUnitAnimation(int Index, AniType Ani)
    {
        if (!UnitAnimationDic.ContainsKey((CommonType)Index) || !UnitAnimationDic[(CommonType)Index].ContainsKey(Ani))
            return null;

        return UnitAnimationDic[(CommonType)Index][Ani];
    }

}
