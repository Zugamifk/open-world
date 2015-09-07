using UnityEngine;
using System;
using System.Collections;

public interface IEditorDrawer {
    // Full method
    EditorGUIx.FieldDrawer GetFieldDrawer(Rect position, string label, IEditorDrawer value, Action<IEditorDrawer> setCallback);
    EditorGUIx.FieldDrawer GetFieldDrawer(Rect position, IEditorDrawer value, Action<IEditorDrawer> setCallback);

    // Layout version
    EditorGUIx.FieldDrawer GetFieldDrawer(string label, IEditorDrawer value, Action<IEditorDrawer> setCallback);
    EditorGUIx.FieldDrawer GetFieldDrawer(IEditorDrawer value, Action<IEditorDrawer> setCallback);

}
