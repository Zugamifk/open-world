#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/** Utility class for managing scriptable objects by path. Only works in the Editor. */
public class EditorCustomAssets {
	
	public const string	kResourcesPath = "Assets/Resources",
						kAssetExtension = ".asset",
						kAssetSearchFilter = "*.asset";
	
	/* Retrieves the scriptable object at the provided path, creating it if it didn't already exist. */
	public static T GetOrCreateAsset<T>(string path, bool autoCreateDirectory = false) where T : ScriptableObject {
		if (!path.EndsWith(kAssetExtension)) {
			throw new System.Exception("Invalid path. Custom assets must use the "+kAssetExtension+" extension");
		}
		T assetObject;
		//Fetch object if it already exists
		if (File.Exists(path)) {
			assetObject = (T)AssetDatabase.LoadMainAssetAtPath(path);
		}
		//Else create it
		else {
			if(autoCreateDirectory && !Directory.Exists(Path.GetDirectoryName(path))){
				new DirectoryInfo(Path.GetDirectoryName(path)).Create();
			}
			assetObject = (T)ScriptableObject.CreateInstance(typeof(T));
			AssetDatabase.CreateAsset(
				assetObject,
				path
			);
		}
		return assetObject;
	}
	
	/* Retrieves the scriptable object with the given name from inside the currently selected directory,
		or next to the currently selected file if the current selection is a file. If the object didn't
		already exist it is created. */
	public static T GetOrCreateAssetOnSelection<T>(string defaultName) where T : ScriptableObject{
		string assetPath;
		//Figure out where to look
		if (AssetDatabase.Contains(Selection.activeObject)) {
			assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (!Directory.Exists(assetPath)) {
				assetPath = Path.GetDirectoryName(assetPath);
			}
		} else {
			assetPath = "Assets";
		}		
		assetPath += "/" + defaultName + kAssetExtension;
		
		//Fetch / create the object there		
		T obj = EditorCustomAssets.GetOrCreateAsset<T>(assetPath);
		
		AssetDatabase.SaveAssets();
		
		Selection.activeObject = obj;
		return obj;
	}
	
	public static List<T> GetAllAssetsAtPath<T>(string dirPath) where T : ScriptableObject {
		string[] filePaths = Directory.GetFiles(
			dirPath,
			kAssetSearchFilter,
			SearchOption.AllDirectories
		);
		
		var foundAssets = new List<T>();
		
		foreach(string filePath in filePaths){
			var asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(T));
			if (asset) {
				foundAssets.Add((T)asset);
			}
		}
		return foundAssets;
	}
}
#endif