using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(GenInfo))]
public class GenInfoSetButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenInfo info = (GenInfo)target;
        if (GUILayout.Button("Set GenInfo"))
        {
            info.SetGenInfo();
        }
    }
}
#endif