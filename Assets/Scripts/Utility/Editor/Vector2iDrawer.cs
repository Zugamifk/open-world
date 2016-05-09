using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Extensions
{
    [CustomPropertyDrawer(typeof(Vector2i))]
    public class Vector2iDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.Next(true);
            EditorGUI.MultiPropertyField(position, new GUIContent[] { new GUIContent("X"), new GUIContent("Y") }, property, label);
        }
    }
}