using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyAttributeDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		ReadonlyAttribute att = attribute as ReadonlyAttribute;
		GUI.enabled = false;
        EditorGUI.PropertyField(position, property, att.label == string.Empty ? label : new GUIContent(att.label));
		GUI.enabled = true;
    }
}
