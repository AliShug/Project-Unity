using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(UniqueIDAttribute))]
public class UniqueIDDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Generate unique ID
        if (property.stringValue == "")
        {
            Guid guid = Guid.NewGuid();
            property.stringValue = guid.ToString();
        }

        // Label it (non-editable)
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        EditorGUI.LabelField(textFieldPosition, label, new GUIContent(property.stringValue));
    }
}
