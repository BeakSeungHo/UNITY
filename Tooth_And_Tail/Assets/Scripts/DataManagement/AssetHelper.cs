#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-24
///  
///  Desc.
///     Asset을 만들어주는 Helper 코드
/// 
/// </summary>


public static class AssetHelper
{

    public static string CreateAsset<T>(string assetName) where T : ScriptableObject
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }

        return CreateAsset<T>(path, assetName);
    }


    public static string CreateAsset<T>(string targetFoler, string assetName) where T : ScriptableObject
    {
        var instance = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(instance, targetFoler + "/" + assetName);
        return targetFoler;
    }
}
#endif
