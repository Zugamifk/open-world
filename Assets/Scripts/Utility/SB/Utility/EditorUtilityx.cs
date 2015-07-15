#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class EditorUtilityx {

	public static void ForEachScene(string baseDirectory, System.Action<string> action){
		foreach(string scenePath in Directory.GetFiles(baseDirectory, "*.unity", SearchOption.AllDirectories)){
			EditorApplication.OpenScene(scenePath);
			action(scenePath);
		}
	}
}
#endif

// Static methods to guard against different cases between playing in editor vs elsewhere
public static class EditorProtection {
	public static bool IsPlaying {
        get
        {
			#if UNITY_EDITOR
			if(!Application.isPlaying) return false;
			#endif
            return true;
        }
    }
}
