using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-24
///  
///  Desc.
///     List를 초기화하는 버튼 커스텀하는 코드
/// 
/// </summary>

#if UNITY_EDITOR
[CustomEditor(typeof(CommonElements))]
public class CommonListRefresh : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CommonElements commonElements = (CommonElements)target;

        if (GUILayout.Button("Refresh List"))
        {
            commonElements.Refresh();
        }
    }
}

[CustomEditor(typeof(UserElements))]
public class UserListRefresh : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UserElements userElements = (UserElements)target;

        if (GUILayout.Button("Refresh List"))
        {
            userElements.Refresh();
        }
    }
}

[CustomEditor(typeof(ReinforceElements))]
public class ReinforceListRefresh : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ReinforceElements ReinforceElements = (ReinforceElements)target;

        if (GUILayout.Button("Refresh List"))
        {
            ReinforceElements.Refresh();
        }
    }
}
#endif