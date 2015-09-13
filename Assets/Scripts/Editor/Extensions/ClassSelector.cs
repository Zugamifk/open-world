using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Lambdas;

public class ClassSelector<T> where T : class {
	private Type current;
	private T instance;
	private int selection;
	private Type[] OptionTypes;
	private Type[] GenericParameters;
	private string[] Options;
	private EditorGUIx.FieldDrawerLayout[] FieldDrawers;
	private bool raisedError = false;
	private bool initialized = false;

	public delegate void OnDelete();
	public OnDelete DeleteEvent;

	public T Instance {
		get { return instance;}
	}

	public ClassSelector(params Type[] genericParams) {
		GenericParameters = genericParams;
		Init();
	}

	public void Init() {

		RefreshOptions();

		if (OptionTypes.Length>0) {
			SetCurrent(OptionTypes[0]);
			Instantiate();
			RefreshDrawers();
		} else {
			FieldDrawers = new EditorGUIx.FieldDrawerLayout[0];
		}

		raisedError = false;
		initialized = true;
	}

	public void Refresh() {
		RefreshOptions();
		SetCurrent(OptionTypes[selection]);
		Instantiate();
		RefreshDrawers();
	}

	public void RefreshOptions() {
		OptionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
			assembly => assembly
				.GetTypes()
				.Where( t =>
				t != typeof(T) &&
				typeof(T).IsAssignableFrom(t) &&
				t.GetConstructor(Type.EmptyTypes)!=null))
			.Select( t => {
				if(t.IsGenericTypeDefinition) {
					if(GenericParameters!=null) {
						return t.MakeGenericType(GenericParameters);
					} else {
						Debug.LogError("Generic Type "+t+" requires a class selector with type arguments!");
						raisedError = true;
						return null;
					}
				} else {
					return t;
				}
				})
			.Where( t => t != null)
				.ToArray();

		Options = OptionTypes.Select(g => g.Name.Uncamel()).ToArray();
	}

	public void RefreshDrawers() {
		// set up fields
		var fields = current.GetFields();
		FieldDrawers = new EditorGUIx.FieldDrawerLayout[fields.Length];
		for(int i=0;i<fields.Length;i++) {
			FieldDrawers[i] = GetFieldDrawer(fields[i]);
		}
		FieldDrawers = FieldDrawers.Where(f=>f!=null).ToArray();
	}

	private EditorGUIx.FieldDrawerLayout GetFieldDrawer(FieldInfo field) {

		var type = field.FieldType;
		var name = field.Name.Uncamel();
		EditorGUIx.FieldDrawerLayout fieldDrawer = EditorGUIx.NullDrawerLayout;
		if (typeof(Array).IsAssignableFrom(type)) {
			object[] value = (object[])field.GetValue(instance);
            bool foldout = false;
            return () => EditorGUIx.ArrayField(name, ref foldout, value, true);
		} else
		if (typeof(IList).IsAssignableFrom(type)) {
            var list = (IList)field.GetValue(instance);
			if (list==null) {
				Debug.LogWarning("Field "+field.Name+" has a list field that has not been instantiated!");
				return null;
			}
			// var elementType = list.GetType().GetProperty("Item").PropertyType;
            fieldDrawer = EditorGUIx.GetReorderableListFieldDrawer(name, list, typeof(object)	);
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
			Refresh();
		}
		if(GUILayout.Button("Reload")) {
			Instantiate();
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

	protected void SetCurrent(Type t) {
		if(t.IsGenericTypeDefinition) {
			if(GenericParameters!=null) {
				current = t.MakeGenericType(GenericParameters);
			} else {
				Debug.LogError("Generic Type "+t+" requires a class selector with type arguments!");
				raisedError = true;
				return;
			}
		} else {
			current = t;
		}
	}

	protected void Instantiate() {
		var constructor = current.GetConstructor(Type.EmptyTypes);
		if(constructor!=null) {
			instance = constructor.Invoke(null) as T;
		} else {
			Debug.LogWarning(current+" has no empty constructor!");
			raisedError = true;
		}
	}

}
