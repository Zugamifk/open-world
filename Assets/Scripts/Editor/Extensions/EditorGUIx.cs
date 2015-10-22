using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class EditorGUIx
{

    // _____________________________________/ A generic list drawer \___________
    public static bool ListField<T>(
        string label,
        ref bool foldout,
        List<T> list,
        bool allowSceneObjects,
        params GUILayoutOption[] options
    )
    {
        foldout = EditorGUILayout.Foldout(foldout, label);
        if (foldout)
        {
            T oldVal = default(T);
            T newVal = default(T);
            bool replace = false;

            foreach (T val in list)
            {
                GetFieldDrawerLayout(typeof(T), () => val, v => {
					newVal = (T)v;
                    oldVal = val;
                    replace = true;
				})();
    			if (replace) break;
            }
            if (replace)
            {
                int index = list.IndexOf(oldVal);
                if (index != -1)
                {
                    list[index] = newVal;
                }
            }
        }
        return foldout;
    }

    // _____________________________________/ A generic array drawer \__________
    public static void ArrayField<T>(
        string label,
        ref bool foldout,
        T[] array,
        bool allowSceneObjects,
        params GUILayoutOption[] options
    )
    {
        foldout = EditorGUILayout.Foldout(foldout, label);
        if (foldout)
        {
            for(int i=0;i<array.Length;i++) {
                var val = array[i];
                GetFieldDrawerLayout(typeof(T), () => val, v => {
                    array[i] = (T)v;
				})();
            }
        }
    }

	// ___________________________________/ A generic reorderablelist drawer \__
	public static FieldDrawerLayout GetReorderableListFieldDrawer(
		string label,
		IList list,
		Type elementType
	)
	{
		var rList = new ReorderableList(list, elementType);

		rList.drawHeaderCallback = rect => GUI.Label(rect, label);

		EditorGUIx.FieldDrawer[] listFieldDrawers = new EditorGUIx.FieldDrawer[0];

        Action refreshFieldDrawers = () =>
        {
            listFieldDrawers = new EditorGUIx.FieldDrawer[rList.count];
            listFieldDrawers.ToEach(
                (_, i) => EditorGUIx.GetFieldDrawer(
                    elementType,
                    () => list[i],
                    v => list[i] = v
                )
            );
        };
        refreshFieldDrawers();

        rList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
				listFieldDrawers[index](rect);

		var constructor = elementType.GetConstructor(Type.EmptyTypes);
        if(!list.IsFixedSize) {
            rList.onAddCallback = rl =>
            {
                var o = constructor != null ?
                    constructor.Invoke(null) :
                    elementType.IsValueType && Nullable.GetUnderlyingType(elementType) == null ?
                    Activator.CreateInstance(elementType) :
                    null;
                list.Add(o);
            };
        } else {
            rList.onAddCallback = rl => {Debug.LogWarning("Type "+elementType+" does not have a default constructor!");};
        }

        rList.onChangedCallback = rl =>
        {
            refreshFieldDrawers();
        };

        return () => rList.DoLayoutList();
    }

    // _____________________________________/ FIELD DRAWER \___________________
    // A collection of overloaded field drawers for editor scripting
    // For each type, the methods are identical except for the types and the
    // GUI drawing funciton called. Surely this can be compressed somehow!

    // A delegate to define the type for our field drawers
	public delegate void FieldDrawerLayout();
    public delegate void FieldDrawer(Rect r);

    // A field drawer to act as null
	public static void NullDrawerLayout() { }
    public static void NullDrawer(Rect r) { }

    // field for a getter
	public delegate object FieldDrawerGetter(Rect r, ref object value);
    public delegate object FieldDrawerGetterLayout(ref object value);

    // field for a setter
    public delegate object FieldDrawerSetter(object value, object newValue);

    //A field drawer getter, given a type
    public static FieldDrawer GetFieldDrawer(
		Type type,
		string label,
		Func<object> valueInit,
		Action<object> setCallback
	) {
		if(type == typeof(float)) {
            float value = (float)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.FloatField(position, value) :
					EditorGUI.FloatField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
		if (type == typeof(int)) {
			int value = (int)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.IntField(position, value) :
					EditorGUI.IntField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
		if (type == typeof(bool)) {
			bool value = (bool)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.Toggle(position, value) :
					EditorGUI.Toggle(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
        if (type == typeof(string)) {
			string value = (string)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.TextField(position, value) :
					EditorGUI.TextField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
        if (type == typeof(AnimationCurve)) {
			AnimationCurve value = (AnimationCurve)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.CurveField(position, value) :
					EditorGUI.CurveField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
        if (type == typeof(Color)) {
			Color value = (Color)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.ColorField(position, value) :
					EditorGUI.ColorField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
        if (type == typeof(Color)) {
			Color value = (Color)valueInit();
            return (Rect position) =>
			{
				var newVal = label == string.Empty ?
					EditorGUI.ColorField(position, value) :
					EditorGUI.ColorField(position, label, value);
				if (newVal != value)
				{
					value = newVal;
					setCallback(value);
				}
			};
		} else
        if (typeof(object[]).IsAssignableFrom(type)) {
			object[] value = (object[])valueInit();
            bool foldout = false;
            return (Rect position) =>ArrayField(label,ref foldout,value,true);
		} else
		if (typeof(IEditorDrawer).IsAssignableFrom(type)) {
            IEditorDrawer initValue = (IEditorDrawer)valueInit();
            return initValue.GetFieldDrawer(label, initValue, v=>setCallback(v));
		} else
        if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
            UnityEngine.Object value = (UnityEngine.Object)valueInit();
            return (Rect position) =>
            {
                var newVal = label == string.Empty ?
                    EditorGUI.ObjectField(position, value, type, true) :
                    EditorGUI.ObjectField(position, label, value, type, true);
                if (newVal != value)
                {
                    value = newVal;
                    setCallback(value);
                }
            };
        } else
		{
			return NullDrawer;
		}
	}

	// TODO: get rid of everythign down here
	public static FieldDrawer GetFieldDrawer(Type type, Func<object> valueInit, Action<object> setCallback)
	{
        return GetFieldDrawer(type, "", valueInit, setCallback);
    }

	//A field drawer getter, given a type (layout version)
	public static FieldDrawerLayout GetFieldDrawerLayout(
		Type type,
		string label,
		Func<object> valueInit,
		Action<object> setCallback
	) {
		if(type == typeof(float)) {

			return EditorGUIx.GetFieldDrawerLayout(label, (float)valueInit(), v=>setCallback(v));
		} else
		if (type == typeof(int)) {
			return EditorGUIx.GetFieldDrawerLayout(label, (int)valueInit(), v=>setCallback(v));
		} else
		if (type == typeof(bool)) {
			return EditorGUIx.GetFieldDrawerLayout(label, (bool)valueInit(), v=>setCallback(v));
		} else
        if (type == typeof(string)) {
			return EditorGUIx.GetFieldDrawerLayout(label, (string)valueInit(), v=>setCallback(v));
		} else
        if(type == typeof(Color)) {
            return EditorGUIx.GetFieldDrawerLayout(label, (Color)valueInit(), v=>setCallback(v));
        } else
        if(type == typeof(AnimationCurve)) {
            return EditorGUIx.GetFieldDrawerLayout(label, (AnimationCurve)valueInit(), v=>setCallback(v));
        } else
        if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
            return EditorGUIx.GetFieldDrawerLayout(label, type, (UnityEngine.Object)valueInit(), v=>setCallback(v));
        } else
        if (typeof(object[]).IsAssignableFrom(type)) {
			object[] value = (object[])valueInit();
            bool foldout = false;
            return () =>ArrayField(label,ref foldout,value, true);
		} else
        if (type.IsEnum) {
            return EditorGUIx.GetFieldDrawerLayout(label, (System.Enum)valueInit(), v=>setCallback(v));
        } else {
			return NullDrawerLayout;
		}
	}
	public static FieldDrawerLayout GetFieldDrawerLayout(Type type, Func<object> valueInit, Action<object> setCallback)
	{
        return GetFieldDrawerLayout(type, "", valueInit, setCallback);
    }

    // _____________________________________/ float drawer \____________________

    // Full method
    public static FieldDrawer GetFieldDrawer(string label, float value, Action<float> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.FloatField(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, float value, Action<float> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.FloatField(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // _____________________________________/ int drawer \______________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, int value, Action<int> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.IntField(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, int value, Action<int> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.IntField(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // _____________________________________/ bool drawer \____________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, bool value, Action<bool> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.Toggle(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, bool value, Action<bool> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.Toggle(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

	// _____________________________________/ Unity Object drawer \_____________
	public static FieldDrawer GetFieldDrawer(string label, Type type, UnityEngine.Object value, Action<UnityEngine.Object> setCallback)
	{
		return (Rect position) =>
		{
			var newVal = EditorGUI.ObjectField(position, label, value, type, true);
			if (newVal != value)
			{
				value = newVal;
				setCallback(value);
			}
		};
	}
	public static FieldDrawer GetFieldDrawer(Type type, UnityEngine.Object value, Action<UnityEngine.Object> setCallback)
	{
		return GetFieldDrawer("", type, value, setCallback);
	}

	// Layout version
	public static FieldDrawerLayout GetFieldDrawerLayout(string label, Type type, UnityEngine.Object value, Action<UnityEngine.Object> setCallback)
	{
		return () =>
		{
			var newVal = EditorGUILayout.ObjectField(label, value, type, true);
			if (newVal != value)
			{
				value = newVal;
				setCallback(value);
			}
		};
	}
	public static FieldDrawerLayout GetFieldDrawerLayout(Type type, UnityEngine.Object value, Action<UnityEngine.Object> setCallback)
	{
		return GetFieldDrawerLayout("", type, value, setCallback);
	}

	// _____________________________________/ Class dropdown drawer \_____________
	public static FieldDrawer GetFieldDrawer(string label, Type value, Action<Type> setCallback)
	{
		var subtypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where( t =>
				t.IsSubclassOf(value))
			.ToArray();
		var typeStrings = subtypes.Select(g => g.Name.Uncamel()).ToArray();
        int selection = 0;
        return (Rect position) =>
		{
			var ns = EditorGUI.Popup(position, label, selection, typeStrings);
			if (ns!=selection) {
				selection = ns;
                value = subtypes[ns];
                setCallback(value);
			}
		};
	}
	public static FieldDrawer GetFieldDrawer(Type value, Action<Type> setCallback)
	{
		return GetFieldDrawer("", value, setCallback);
	}

	// Layout version
	public static FieldDrawerLayout GetFieldDrawerLayout(string label, Type value, Action<Type> setCallback)
	{
		var subtypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where( t =>
				t.IsSubclassOf(value))
			.ToArray();
		var typeStrings = subtypes.Select(g => g.Name.Uncamel()).ToArray();
        int selection = 0;
        return () =>
		{
			var ns = EditorGUILayout.Popup(label, selection, typeStrings);
			if (ns!=selection) {
				selection = ns;
                value = subtypes[ns];
                setCallback(value);
			}
		};
	}
	public static FieldDrawerLayout GetFieldDrawerLayout(Type value, Action<Type> setCallback)
	{
		return GetFieldDrawerLayout("", value, setCallback);
	}

    // _____________________________________/ enum drawer \_____________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, Enum value, Action<Enum> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.EnumPopup(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, Enum value, Action<Enum> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.EnumPopup(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // _____________________________________/ Color drawer \____________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, Color value, Action<Color> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.ColorField(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, Color value, Action<Color> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.ColorField(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // _____________________________________/ curve drawer \____________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, AnimationCurve value, Action<AnimationCurve> setCallback)
    {
        if (value == null)
        {
            value = AnimationCurve.Linear(0, 1, 1, 1);
            setCallback(value);
        }
        return (Rect position) =>
        {
            var newVal = EditorGUI.CurveField(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, AnimationCurve value, Action<AnimationCurve> setCallback)
    {
        if (value == null)
        {
            value = AnimationCurve.Linear(0, 1, 1, 1);
            setCallback(value);
        }
        return () =>
        {
            var newVal = EditorGUILayout.CurveField(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // _____________________________________/ string drawer \____________________
    // Full method
    public static FieldDrawer GetFieldDrawer(string label, string value, Action<string> setCallback)
    {
        return (Rect position) =>
        {
            var newVal = EditorGUI.TextField(position, label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }

    // Layout version
    public static FieldDrawerLayout GetFieldDrawerLayout(string label, string value, Action<string> setCallback)
    {
        return () =>
        {
            var newVal = EditorGUILayout.TextField(label, value);
            if (newVal != value)
            {
                value = newVal;
                setCallback(value);
            }
        };
    }
}
