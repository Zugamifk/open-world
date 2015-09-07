using UnityEngine;
using System;
using System.Collections;

public class NullEditorDrawer : IEditorDrawer {

	public virtual EditorGUIx.FieldDrawer GetFieldDrawer(Rect position, string label, IEditorDrawer value, Action<IEditorDrawer> setCallback) {
		return EditorGUIx.NullDrawer;
	}
	public virtual EditorGUIx.FieldDrawer GetFieldDrawer(Rect position, IEditorDrawer value, Action<IEditorDrawer> setCallback) {
		return GetFieldDrawer(position, "", value, setCallback);
	}

	// Layout version
	public virtual EditorGUIx.FieldDrawer GetFieldDrawer(string label, IEditorDrawer value, Action<IEditorDrawer> setCallback){
		return EditorGUIx.NullDrawer;
	}
	public virtual EditorGUIx.FieldDrawer GetFieldDrawer(IEditorDrawer value, Action<IEditorDrawer> setCallback) {
		return GetFieldDrawer("", value, setCallback);
	}
}
