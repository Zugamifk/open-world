using UnityEngine;
using UnityEditor;

public class ToggleActive {
	
	[MenuItem("GameObject/Toggle Active %e")]
	public static void ToggleSelectionActive() {
		foreach (Transform t in Selection.transforms) {
			t.gameObject.SetActive(!t.gameObject.activeSelf);
		}
	}

	[MenuItem("GameObject/Make Active Recursively &%e")]
	public static void MakeActiveRecursively() {
		foreach (Transform t in Selection.transforms) {
			SetActiveTrueRecursively(t);
		}
	}
	
	protected static void SetActiveTrueRecursively(Transform parent) {
		parent.gameObject.SetActive(true);
		foreach (Transform child in parent) {
			SetActiveTrueRecursively(child);
		}
	}

	[MenuItem("GameObject/Make Active Recursively &%e", true)]
	[MenuItem("GameObject/Toggle Active %e", true)]
	public static bool ValidateToggleSelection() {
		return Selection.activeTransform;
	}
}