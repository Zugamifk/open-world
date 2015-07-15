using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class AssetDatabasex {
	
	protected const string kAssetDirectory = "Assets";
	
	public static string ProjectPath {
		get {
			if (m_projectPath == null) {
				m_projectPath = Application.dataPath.Substring(
					0,
					Application.dataPath.LastIndexOf(kAssetDirectory)
				);
			}
			return m_projectPath;
		}
	}
	protected static string m_projectPath;
	
	public static string GetProjectRelativePath(string absolutePath) {
		if (!absolutePath.StartsWith(ProjectPath)) {
			throw new ArgumentException("Path is not in current project: " + absolutePath);
		} else {
			return absolutePath.Substring(ProjectPath.Length);
		}
	}

	//Adds an asset label to an object if it does not already possess that label
	//Returns true if the asset did not already possess the label, false otherwise.
	public static bool AddLabel(UnityEngine.Object obj, string label){
		string[] labels = AssetDatabase.GetLabels(obj);
		if(System.Array.Exists(labels, delegate(string s){return s == label;})){
			return false;
		}
		else{
			labels = ArrayExtensions.InsertAt(labels, label, labels.Length);
			AssetDatabase.SetLabels(obj, labels);
			return true;
		}
	}

	//Removes an asset label from an object. Returns true if the object possessed the label, false otherwise.
	public static bool RemoveLabel(UnityEngine.Object obj, string label){
		string[] labels = AssetDatabase.GetLabels(obj);
		int index = System.Array.FindIndex(labels, delegate(string s){return s == label;});
		if(index >= 0){
			labels = ArrayExtensions.RemoveAt(labels, index);
			AssetDatabase.SetLabels(obj, labels);
			return true;
		}
		else{
			return false;
		}
	}
	
	public static bool HasLabel(UnityEngine.Object obj, string label){
		string[] labels = AssetDatabase.GetLabels(obj);
		return System.Array.Exists(labels, delegate(string s){return s.Equals(label, System.StringComparison.OrdinalIgnoreCase);});
	}
}
