using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnitTestWindow : EditorWindow {

	[MenuItem("Window/Unit Test")]
	public static void Open() {
		UnitTestWindow window = (UnitTestWindow)EditorWindow.GetWindow (typeof (UnitTestWindow));
		window.Refresh();
		window.Show();
	}

	private ClassSelector<IUnitTestable> Selector;
	private IUnitTestable unitTest;

	void Init() {
		Selector = new ClassSelector<IUnitTestable>(typeof(int));
	}

	void Refresh() {
		if(Selector!=null) {
			Selector.Refresh();
		} else {
			Init();
		}
	}

	void OnEnable() {
		Refresh();
	}

	void OnProjectChange() {
		Refresh();
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

        GUILayout.Space(25);
		if(GUILayout.Button("Run Mini Test")) {
            MiniTest();
        }
    }


	void MiniTest() {

  }
}
