﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Linq;
using MeshGenerator;

public class MeshGeneratorWindow : EditorWindow {

	const string kModelPath = "Assets/Models/";

	Mesh CurrentMeshAsset;
	IMeshGenerator CurrentGenerator;
	EditorGUIx.FieldDrawerLayout[] GeneratorFieldDrawers;
	Type[] Generators;
    Action<Mesh>[] ColorFunctions;
    Action[] ColorDrawFunctions;
    int colorSelection = 0;

    [MenuItem("Window/Mesh Generator")]
	public static void Open() {
		MeshGeneratorWindow window = (MeshGeneratorWindow)EditorWindow.GetWindow (typeof (MeshGeneratorWindow));
		window.Init();
		window.Show();
	}

	private EditorGUIx.FieldDrawerLayout GetFieldDrawer(FieldInfo field, IMeshGenerator generator) {

		var type = field.FieldType;
		var name = field.Name.Uncamel();
		EditorGUIx.FieldDrawerLayout fieldDrawer = EditorGUIx.NullDrawerLayout;
		if (typeof(IList).IsAssignableFrom(type)) {
            var list = (IList)field.GetValue(generator);
			if(list == null) {
				Debug.LogWarning("list "+field.Name+" has not been instantiated");
				return EditorGUIx.NullDrawerLayout;
			}
			Type elementType = null;
			bool found = Linqx.TryListOfWhat(list.GetType(), out elementType);
			if(!found) {
				Debug.LogWarning("list "+field.Name+" does not appear to implement IList!");
				return null;
			}
            fieldDrawer = EditorGUIx.GetReorderableListFieldDrawer(name, list, elementType);
        } else {
			fieldDrawer = EditorGUIx.GetFieldDrawerLayout(
				type,
				name + ":",
				() => field.GetValue(generator),
				v => field.SetValue(generator, v)
			);
		}

        return fieldDrawer;
    }

	IMeshGenerator NewGenerator(Type generator) {
		var constructor = generator.GetConstructor(Type.EmptyTypes);

		IMeshGenerator gen = null;
		if(constructor!=null) {
			gen = constructor.Invoke(null) as IMeshGenerator;
		} else {
			Debug.LogWarning(generator+" has no empty constructor!");
			return gen;
		}

		// set up fields
		var fields = generator.GetFields();
		GeneratorFieldDrawers = new EditorGUIx.FieldDrawerLayout[fields.Length];
		for(int i=0;i<fields.Length;i++) {
			GeneratorFieldDrawers[i] = GetFieldDrawer(fields[i], gen);
		}
		return gen;
	}

	void RegenerateMesh() {

		// Generate mesh
		Mesh mesh = CurrentGenerator.Generate();

		// Generate normals and optimize
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		// Update/save mesh asset
		if(CurrentMeshAsset!=null) {
			CurrentMeshAsset.vertices = mesh.vertices;
			CurrentMeshAsset.triangles = mesh.triangles;
			CurrentMeshAsset.uv = mesh.uv;
			CurrentMeshAsset.normals = mesh.normals;
			CurrentMeshAsset.colors = mesh.colors;
			CurrentMeshAsset.tangents = mesh.tangents;
            ColorFunctions[colorSelection](mesh);
            EditorUtility.SetDirty(CurrentMeshAsset);
			AssetDatabase.SaveAssets();
		} else {
			AssetDatabase.CreateAsset(mesh, kModelPath+"New Mesh.asset");
			CurrentMeshAsset = mesh;
		}
		Debug.Log("Generated mesh "+mesh.name+" with "+CurrentGenerator.Name);
	}

	void Init() {

		Generators = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
			assembly => assembly
		    .GetTypes()
		    .Where( t =>
				t != typeof(IMeshGenerator) &&
				typeof(IMeshGenerator).IsAssignableFrom(t)))
		    .ToArray();

		generatorOptions = Generators.Select(g => g.Name.Uncamel()).ToArray();

		if (Generators.Length>0) {
			CurrentGenerator = NewGenerator(Generators[0]);
		}

		Color a = Color.black;
		Color b = Color.black;
        ColorDrawFunctions = new Action[] {
            ()=>{},
			()=>{
                EditorGUILayout.BeginHorizontal();
                a = EditorGUILayout.ColorField(a);
                b = EditorGUILayout.ColorField(b);
				EditorGUILayout.EndHorizontal();
            },
			()=>{
                EditorGUILayout.BeginHorizontal();
                a = EditorGUILayout.ColorField(a);
                b = EditorGUILayout.ColorField(b);
				EditorGUILayout.EndHorizontal();
            }
        };
        ColorFunctions = new Action<Mesh>[] {
            m => {},
			m => m.ColorByVertexIndex(a, b),
			m => m.ColorByLastTriangleIndex(a, b)
        };
    }

	void OnProjectChange() {
		Init();
	}

	// selector vars
	int generatorSelection = 0;
	string[] generatorOptions = new string[]{};
	void OnGUI() {
		EditorGUILayout.BeginHorizontal();
		var gs = EditorGUILayout.Popup("Generator:", generatorSelection, generatorOptions);
		if (gs!=generatorSelection || CurrentGenerator == null) {
			if(Generators == null) Init();
			generatorSelection = gs;
			CurrentGenerator = NewGenerator(Generators[gs]);
		}
		if(GUILayout.Button("Reload")) {
			CurrentGenerator = NewGenerator(Generators[gs]);
		}
		EditorGUILayout.EndHorizontal();

		var ma = (Mesh)EditorGUILayout.ObjectField("Asset:", CurrentMeshAsset, typeof(Mesh), false);
		if (ma!=CurrentMeshAsset) {
			CurrentMeshAsset = ma;
		}

        ColorDrawFunctions[colorSelection]();
        colorSelection = GUILayout.SelectionGrid(colorSelection, new string[] { "None", "Vertex", "Tris" }, 3);

        var gen = CurrentGenerator;

		if (gen!=null && CurrentMeshAsset != null) {
			GUILayout.Label(CurrentMeshAsset.name + " -- " + gen.Name);
			foreach(var f in GeneratorFieldDrawers) if(f!=null)f();

			if(GUILayout.Button("Generate Mesh!")) {
				RegenerateMesh();
			}
		} else if (
			CurrentMeshAsset == null &&
			GUILayout.Button("Create New"))
		{
			var asset = new Mesh ();
	        AssetDatabase.CreateAsset(asset, kModelPath+"New Mesh.asset");
	        if(AssetDatabase.Contains(asset)) {
				CurrentMeshAsset = asset;
			}
		}
	}

}
