using UnityEditor;
using UnityEngine;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-24
///  
///  Desc.
///     List에 있는 Attribute의 Element Title을 Enum값을 가져와서 변경하는 코드
/// 
/// 
/// </summary>



//Attribute
public class ArrayElementTitleAttribute : PropertyAttribute
{
    public string Varname;
    public ArrayElementTitleAttribute(string ElementTitleVar)
    {
        Varname = ElementTitleVar;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
public class ArrayElementTitleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementTitleAttribute Attribute => attribute as ArrayElementTitleAttribute;
    SerializedProperty TitleNameProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string FullPathName = property.propertyPath + "." + Attribute.Varname;
        TitleNameProp = property.serializedObject.FindProperty(FullPathName);
        string newLabel = GetTitle();
        if (string.IsNullOrEmpty(newLabel))
            newLabel = label.text;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
    }

    string GetTitle()
    {
        return TitleNameProp.enumNames[TitleNameProp.enumValueIndex];
    }
}

#endif