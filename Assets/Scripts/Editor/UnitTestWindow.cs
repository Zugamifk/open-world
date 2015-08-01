using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnitTestWindow : EditorWindow {

	[MenuItem("Window/Unit Test")]
	public static void Open() {
		UnitTestWindow window = (UnitTestWindow)EditorWindow.GetWindow (typeof (UnitTestWindow));
		window.Init();
		window.Show();
	}

	private ClassSelector<IUnitTestable> Selector;
	private IUnitTestable unitTest;

	void Init() {
		Selector = new ClassSelector<IUnitTestable>(typeof(int));
	}

	void OnEnable() {
		Init();
	}

	void OnGUI() {
		if(Selector!=null)
			unitTest = Selector.DrawField(unitTest);
		var enabled = unitTest!=null;
		GUI.enabled = enabled;
		if(GUILayout.Button("Test!")) {
			unitTest.Test();
		}
		GUI.enabled = true;
	}
}
