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

	void OnProjectChange() {
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

        GUILayout.Space(25);
		if(GUILayout.Button("Run Mini Test")) {
            MiniTest();
        }
    }

	private struct ttt {
        public string a, b;
    }
	void MiniTest() {
        var tt = new ttt[] {
            new ttt {a = "a", b = "b"},
            new ttt {a="a0", b="b0"}
        };
    	var tt2 = new ttt[2];
        tt2[0] = tt[0];
        tt2[0].a = "c";
        Debug.Log(tt[0].a+":"+tt2[0].a);
    }
}
