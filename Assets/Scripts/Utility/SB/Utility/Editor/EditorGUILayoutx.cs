using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public static class EditorGUILayoutx {
	
#if UNITY_3_3
	
	public static T ObjectField<T>(T obj, params GUILayoutOption[] options) where T : UnityEngine.Object{
		return EditorGUILayout.ObjectField(obj, typeof(T), options) as T;
	}	

	public static T ObjectField<T>(string label, T obj, params GUILayoutOption[] options) where T : UnityEngine.Object{
		return EditorGUILayout.ObjectField(label, obj, typeof(T), options) as T;
	}
	
#else //UNITY_3_3
	
	public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object{
		return EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options) as T;
	}	

	public static T ObjectField<T>(string label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : UnityEngine.Object{
		return EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options) as T;
	}
	
#endif //UNITY_3_3
	
	public static void AutoField(System.Object obj, FieldInfo field){
		AutoField(obj, field, true);
	}
	
	public static void AutoField(System.Object obj, FieldInfo field, bool allowSceneObjects){
		if(field.FieldType == typeof(int)){
			field.SetValue(obj, EditorGUILayout.IntField(ObjectNames.NicifyVariableName(field.Name), (int)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(float)){
			field.SetValue(obj, EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(field.Name), (float)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(bool)){
			field.SetValue(obj, EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(field.Name), (bool)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(string)){
			field.SetValue(obj, EditorGUILayout.TextField(ObjectNames.NicifyVariableName(field.Name), (string)field.GetValue(obj)));
		}
		else if(typeof(Object).IsAssignableFrom(field.FieldType)){
			field.SetValue(obj, EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(field.Name), (Object)field.GetValue(obj), field.FieldType, allowSceneObjects));
		}
		else if(field.FieldType == typeof(Vector2)){
			field.SetValue(obj, EditorGUILayout.Vector2Field(ObjectNames.NicifyVariableName(field.Name), (Vector2)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(Vector3)){
			field.SetValue(obj, EditorGUILayout.Vector3Field(ObjectNames.NicifyVariableName(field.Name), (Vector3)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(Vector4)){
			field.SetValue(obj, EditorGUILayout.Vector4Field(ObjectNames.NicifyVariableName(field.Name), (Vector4)field.GetValue(obj)));
		}
		else if(field.FieldType.IsEnum){
			field.SetValue(obj, EditorGUILayout.EnumPopup(ObjectNames.NicifyVariableName(field.Name), (System.Enum)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(Rect)){
			field.SetValue(obj, EditorGUILayout.RectField(ObjectNames.NicifyVariableName(field.Name), (Rect)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(Bounds)){
			field.SetValue(obj, EditorGUILayout.BoundsField(ObjectNames.NicifyVariableName(field.Name), (Bounds)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(Color)){
			field.SetValue(obj, EditorGUILayout.ColorField(ObjectNames.NicifyVariableName(field.Name), (Color)field.GetValue(obj)));
		}
		else if(field.FieldType == typeof(AnimationCurve)){
			field.SetValue(obj, EditorGUILayout.CurveField(ObjectNames.NicifyVariableName(field.Name), (AnimationCurve)field.GetValue(obj)));
		}		
		else{
			//Fall-back
			GUILayout.Label(ObjectNames.NicifyVariableName(field.Name) + " (no serializer)");
		}
	}
	
	public delegate void SelectCallback(int index);

	public static int SelectionList(int selected, string[] list, GUIStyle elementStyle, SelectCallback callback) {
		for (int i = 0; i < list.Length; ++i) {
			Rect elementRect = GUILayoutUtility.GetRect(new GUIContent(list[i]), elementStyle);
			bool hover = elementRect.Contains(Event.current.mousePosition);
			if (hover && Event.current.type == EventType.MouseDown) {
				selected = i;
				Event.current.Use();
				if(callback != null) {
					callback(i);
				}
			} else if (Event.current.type == EventType.repaint) {
				elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
			}
		}
		return selected;
	}
}
