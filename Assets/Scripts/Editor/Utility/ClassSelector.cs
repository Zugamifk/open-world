using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ClassSelector<T> where T : class {
	private Type current;
	private T instance;
	private int selection;
	private Type[] OptionTypes;
	private string[] Options;
	private EditorGUIx.FieldDrawerLayout[] FieldDrawers;
	private bool raisedError = false;
	private bool initialized = false;

	public delegate void OnDelete();
	public OnDelete DeleteEvent;

	public T Instance {
		get { return instance;}
	}

	public ClassSelector() {
		Init();
		// Debug.Log("Mesh Generator window refreshed");
	}

	public void Init() {
		OptionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
			assembly => assembly
		    .GetTypes()
		    .Where( t =>
				t != typeof(T) &&
				typeof(T).IsAssignableFrom(t)))
		    .ToArray();

		Options = OptionTypes.Select(g => g.Name.Uncamel()).ToArray();

		if (OptionTypes.Length>0) {
			var constructor = OptionTypes[0].GetConstructor(Type.EmptyTypes);
			current = OptionTypes[0];
			if(constructor!=null) {
				instance = constructor.Invoke(null) as T;
			} else {
				Debug.LogWarning(current+" has no empty constructor!");
				raisedError = true;
				return;
			}

			RefreshDrawers();
		} else {
			FieldDrawers = new EditorGUIx.FieldDrawerLayout[0];
		}

		raisedError = false;
		initialized = true;
	}

	public void RefreshDrawers() {
		// set up fields
		var fields = current.GetFields();
		FieldDrawers = new EditorGUIx.FieldDrawerLayout[fields.Length];
		for(int i=0;i<fields.Length;i++) {
			FieldDrawers[i] = GetFieldDrawer(fields[i]);
		}
	}

	private EditorGUIx.FieldDrawerLayout GetFieldDrawer(FieldInfo field) {

		var type = field.FieldType;
		var name = field.Name.Uncamel();
		EditorGUIx.FieldDrawerLayout fieldDrawer = EditorGUIx.NullDrawerLayout;
		if (typeof(IList).IsAssignableFrom(type)) {
            var list = (IList)field.GetValue(instance);
			var elementType = list.GetType().GetProperty("Item").PropertyType;
            fieldDrawer = EditorGUIx.GetReorderableListFieldDrawer(name, list, elementType);
        } else {
			fieldDrawer = EditorGUIx.GetFieldDrawerLayout(
				type,
				name + ":",
				() => field.GetValue(instance),
				v => field.SetValue(instance, v)
			);
		}

        return fieldDrawer;
    }

	public T DrawField(T value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(typeof(T)+": "+value);
		if(!initialized) return default(T);
		var nt = EditorGUILayout.Popup(selection, Options);
		if (!raisedError &&
			(nt!=selection || current == null)) {
			if(OptionTypes == null) Init();
			if(OptionTypes==null || OptionTypes.Length == 0) {
				raisedError = true;
				EditorGUILayout.EndHorizontal();
				return default(T);
			}
			selection = nt;
			current = OptionTypes[nt];
			var constructor = current.GetConstructor(Type.EmptyTypes);
			if(constructor!=null) {
				instance = constructor.Invoke(null) as T;
			} else {
				Debug.LogWarning(current+" has no empty constructor!");
				raisedError = true;
				EditorGUILayout.EndHorizontal();
				return default(T);
			}
		}
		if(DeleteEvent!=null && GUILayout.Button("X", GUILayout.Width(25))) {
			DeleteEvent();
		}
		EditorGUILayout.EndHorizontal();
		if(FieldDrawers!=null) {
			foreach(var f in FieldDrawers) f();
		}
		return instance;
	}

}
