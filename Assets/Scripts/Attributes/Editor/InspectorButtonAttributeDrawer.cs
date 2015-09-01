using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorbuttonAttributeDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 36;
    }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		InspectorButtonAttribute att = attribute as InspectorButtonAttribute;

		var rect = position;
        rect.height = 16;

        GUI.enabled = false;
        EditorGUI.PropertyField(rect, property);
		GUI.enabled = true;

        rect.y += 20;
		var num = att.methodNames.Count;
		rect.width/=num;
        for (int i = 0; i < num; i++)
        {
            if (!string.IsNullOrEmpty(att.buttonLabels[i])) label.text = att.buttonLabels[i];
            if (GUI.Button(rect, label))
            {
                var obj = property.serializedObject;
                if (obj.targetObject is MonoBehaviour)
                {
                    Type type = obj.targetObject.GetType();
                    MethodInfo info = type.GetMethod(att.methodNames[i], BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
                    if (info != null)
                    {
                        fieldInfo.SetValue(obj.targetObject, info.Invoke(obj.targetObject, null));
                    }
                    else
                    {
                        Debug.LogErrorFormat("Method \"{0}\" not a member of type \"{1}\"", att.methodNames[i], type.Name);
                    }
                }
            }
            rect.x += rect.width;
        }
    }
}
