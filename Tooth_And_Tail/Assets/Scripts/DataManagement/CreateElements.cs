using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-24
///  
///  Desc.
///     Unity의 메뉴를 눌러서 Elements Asset을 생성하는 코드
/// 
/// </summary>

#if UNITY_EDITOR
public class CreateElements : MonoBehaviour
{
    [UnityEditor.MenuItem("Mansion/CommonElements 생성")]
    public static void CreateCommonElements()
    {
        AssetHelper.CreateAsset<CommonElements>("CommonElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/UserElements 생성")]
    public static void CreateUserElements()
    {
        AssetHelper.CreateAsset<UserElements>("UserElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/AnimatorElements 생성")]
    public static void CreateAnimatorElements()
    {
        AssetHelper.CreateAsset<AnimatorElements>("AnimatorElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/GameElements 생성")]
    public static void CreateGameElements()
    {
        AssetHelper.CreateAsset<GameElements>("GameElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/TintElements 생성")]
    public static void CreateTintElements()
    {
        AssetHelper.CreateAsset<TintElements>("TintElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/UIElements 생성")]
    public static void CreateUIElements()
    {
        AssetHelper.CreateAsset<UIElements>("UIElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/ReinforceElements 생성")]
    public static void CreateReinforceElements()
    {
        AssetHelper.CreateAsset<ReinforceElements>("ReinforceElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/StatisticElements 생성")]
    public static void CreateStatisticElements()
    {
        AssetHelper.CreateAsset<StatisticElements>("StatisticElements.asset");
    }

    [UnityEditor.MenuItem("Mansion/SoundElements 생성")]
    public static void CreateSoundElements()
    {
        AssetHelper.CreateAsset<SoundElements>("SoundElements.asset");
    }

}
#endif
