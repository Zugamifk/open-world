using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(BinaryAttribute))]
public class BinaryAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var toStr = System.Convert.ToString(property.intValue, 2).PadLeft(8, '0');
        var oldVal = property.intValue;
        var val = EditorGUI.TextField(position, label.text, toStr);
        try
        {
            var toInt = System.Convert.ToInt32(val, 2);
            property.intValue = toInt;
        }
        catch (System.Exception e)
        {
            property.intValue = oldVal;
        }
    }
}
