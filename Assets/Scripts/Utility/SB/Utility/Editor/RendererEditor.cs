using UnityEngine;
using UnityEditor;

public class RendererEditor {
	
	[MenuItem("Edit/Toggle Renderers Recursively %#r", false, 1000)]
	public static void ToggleRenderers() {
		
		var gameObject = Selection.activeGameObject;
		
		bool foundARenderer = false;
		bool newRendererValue = false;
		
		var renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		Undo.RecordObjects(renderers, "Toggle Renderers");
		
		foreach(Renderer r in renderers){
			if( ! foundARenderer){
				newRendererValue = !r.enabled;
				foundARenderer = true;
			}
			r.enabled = newRendererValue;
		}
		
	}
		
}
