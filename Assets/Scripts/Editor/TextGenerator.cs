﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class TextGenerator : EditorWindow
{
    const string kTextFilePath = "Assets/Text/";

    private class WordList {
        public string name;
        public List<string> words;
		public WordList(string name, params string[] words) {
			this.name = name;
            this.words = new List<string>();
        	words.ForEach(w => this.words.Add(w));
		}
		public void Draw() {
            EditorGUILayout.BeginHorizontal();
            var n = EditorGUILayout.TextField(name);
			name = n;

			if(GUILayout.Button("+")) {
                words.Add("");
            }
			EditorGUILayout.EndHorizontal();

			int index = -1;
            bool toRemove = false;
            string toReplace = "";
            words.ForEach((word, i) =>
           	{
               	EditorGUILayout.BeginHorizontal();
               	var w = EditorGUILayout.TextField(word);
	            if (w != word)
	            {
	                index = i;
	        		toReplace = w;
	            }

                if (GUILayout.Button("-"))
               	{
                   index = i;
                   toRemove = true;
               	}
               	EditorGUILayout.EndHorizontal();
           	});
			if(index > -1) {
				if(toRemove) {
					words.RemoveAt(index);
				} else {
					words[index] = toReplace;
				}
			}
        }
    }

	[MenuItem("Window/Text Generator")]
	public static void Open() {
		TextGenerator window = (TextGenerator)EditorWindow.GetWindow (typeof (TextGenerator));
		window.Init();
		window.Show();
	}


    private string fileName = "";
    private string formatString;
    private List<WordList> options = new List<WordList>();

    public void Init() {
        formatString = "";
        options = new List<WordList>();
	}

	public void Generate() {
        var path = kTextFilePath + fileName + ".txt";
        if (File.Exists(path))
		{
			Debug.Log(path +" already exists.");
			return;
		}
		var sr = File.CreateText(path);
        formatString.EnumerableFormat(
			options.Select(wl => wl.words.ToArray()).ToArray()
			).ForEach(
				line => sr.WriteLine(line)
			);

        sr.WriteLine();
		sr.Close();
    }

    private Vector2 scroll;
    void OnGUI() {
        fileName = EditorGUILayout.TextField("File name:", fileName);

        EditorGUILayout.LabelField("Format String");
		EditorGUILayout.BeginHorizontal();
        var fs = EditorGUILayout.TextArea(formatString, GUILayout.Height(100));
		formatString = fs;
		if(GUILayout.Button("Generate!", GUILayout.Width(100), GUILayout.Height(100))) {
            Generate();
        }
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(25);

		EditorGUILayout.LabelField("Argument Lists");

        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach(var o in options) {
			o.Draw();
			GUILayout.Space(10);
		}
		EditorGUILayout.EndScrollView();

        if(GUILayout.Button("New")) {
			options.Add(new WordList("WordList "+options.Count));
		}


    }
}
