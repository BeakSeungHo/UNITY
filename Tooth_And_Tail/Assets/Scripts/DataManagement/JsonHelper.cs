using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-24
///  
///  Desc.
///     Json에서 배열을 저장할때 사용하는 Helper코드
/// 
/// </summary>

public static class JsonHelper {
    
    //데이터를 가져와서 배열에 넣을때 사용
    public static T[] FromJsonArray<T>(string json)
    {
        wrapper<T> wrapper = JsonUtility.FromJson<wrapper<T>>(json);
        return wrapper.items;
    }
    
    //데이터 배열을 가져와서 저장할때 사용
    public static string ToJson<T>(T[] array)
    {
        wrapper<T> wrapper = new wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }
    
    //데이터 배열을 가져와서 가독성있게 출력할때 사용
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        wrapper<T> wrapper = new wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper,prettyPrint);
    }
    
    [System.Serializable]
    private class wrapper<T>
    {        
        public T[] items;
    }
}
