using UnityEngine;
using UnityEditor;

public class TransformEditor {

	[MenuItem("Edit/Copy Transform %&c", false, 1000)]
	public static void CopyTransform() {
		SaveVector3("Position", Selection.activeTransform.localPosition);
		SaveVector3("Rotation", Selection.activeTransform.localEulerAngles);
		SaveVector3("Scale", Selection.activeTransform.localScale);
	}

	[MenuItem("Edit/Copy Transform %&c", true, 1000)]
	public static bool ValidateCopyTransform() {
		return Selection.activeTransform;
	}

	[MenuItem("Edit/Paste Transform %&v", false, 1005)]
	public static void PasteTransform() {
		Undo.RecordObject(Selection.activeTransform, "Paste Transform");
		Selection.activeTransform.localPosition = LoadVector3("Position");
		Selection.activeTransform.localEulerAngles = LoadVector3("Rotation");
		Selection.activeTransform.localScale = LoadVector3("Scale");
	}

	[MenuItem("Edit/Paste Transform %&v", true, 1005)]
	public static bool ValidatePasteTransform() {
		return	Selection.activeTransform &&
				EditorPrefs.HasKey("TransformEditor Clipboard Position0");
	}

	[MenuItem("Edit/Reset Transform %&z", false, 1010)]
	public static void ZeroTransform() {
        Selection.activeTransform.Zero();
    }

	[MenuItem("Edit/Reset Transform %&z", true, 1010)]
	public static bool ValidateZeroTransform() {
        return Selection.activeTransform;
    }

	protected static void SaveVector3(string key, Vector3 v) {
		for (int i = 0; i < 3; ++i) {
			EditorPrefs.SetFloat("TransformEditor Clipboard " + key + i, v[i]);
		}
	}

	protected static Vector3 LoadVector3(string key) {
		Vector3 v = Vector3.zero;
		for (int i = 0; i < 3; ++i) {
			if (!EditorPrefs.HasKey("TransformEditor Clipboard " + key + i)) {
				continue;
			}
			v[i] = EditorPrefs.GetFloat("TransformEditor Clipboard " + key + i);
		}
		return v;
	}

}
