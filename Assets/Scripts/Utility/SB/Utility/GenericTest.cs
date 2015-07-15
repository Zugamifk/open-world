using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Linq;

#if UNITY_EDITOR
public class Tester : EditorWindow {
	[MenuItem("Custom/Test")]
	static protected void ShowWindow () {
		EditorWindow.GetWindow(typeof(Tester));
	}

	GameObject tester = null;
	ITestable test = null;

	protected void OnGUI() {
		var t = EditorGUILayout.ObjectField(tester, typeof(GameObject), true) as GameObject;
		if(t!=tester) {
			tester = t;
			if(t!=null)
				test = t.GetComponentInChildren(typeof(ITestable)) as ITestable;
			else
				test = null;
		}

		if(test!=null && GUILayout.Button("Test!"))
			test.Test();

	}

}
#endif

public interface ITestable {
	void Test();
}

// put small tests or whatever here
public class GenericTest : MonoBehaviour, ITestable {
	public void Test() {
		var ints = Enumerable.Range(0,100);
		int[] cts = new int[100];
		for(int i=0;i<10000;i++) {
			cts[ints.Random()]++;
		}
	}
}
