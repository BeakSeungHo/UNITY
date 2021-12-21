using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-31
///  
///  Desc.
///     필요한 추가 함수들을 작성한곳
/// 
/// 
/// </summary>

public static class ExMethods {
  
    //Resources폴더 안의 Elements폴더에 있는 name에 해당하는 것을 가져와서 T로 반환
    public static T GetElement<T>(string name) where T : ScriptableObject
    {  
        return Resources.Load<T>("Elements/" + name);
    }
    
    //String을 가져와서 Enum값에서 찾아서 int로 반환하는 코드
    public static int ToEnum<T>(this string str)
    {
        if (!System.Enum.IsDefined(typeof(T), str))
            return -1;

        return (int)System.Enum.Parse(typeof(T), str, true);
    }

    public static string[] CSVReader(string FilePath)
    {
        TextAsset DataText = Resources.Load<TextAsset>(FilePath);

        string[] data = DataText.text.Split('\n');

        return data;
    }
}
