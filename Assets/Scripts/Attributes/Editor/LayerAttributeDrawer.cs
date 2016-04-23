using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributeDrawer : PropertyDrawer
{
    static string[] layerNames = GetSortingLayerNames();
    static int[] layerIDs = GetSortingLayerUniqueIDs();
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var id = property.intValue;
        var selection = 0;
        for (int i = 0; i < layerIDs.Length; i++)
        {
            if (layerIDs[i] == id)
            {
                selection = i;
                break;
            }
        }
        var ns= EditorGUI.Popup(position, label.text, selection, layerNames);
        if (selection != ns)
        {
            property.intValue = layerIDs[ns];
        }
    }

    // Get the sorting layer names
    static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    // Get the unique sorting layer IDs -- tossed this in for good measure
    static int[] GetSortingLayerUniqueIDs()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }
}
