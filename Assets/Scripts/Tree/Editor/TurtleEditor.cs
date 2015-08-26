using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(Turtle), true)]
public class TurtleEditor : Editor
{
    string newProduction = "";
    string newProductionPred = "";
    public override void OnInspectorGUI()
    {
        SerializedObject turtleSO = new SerializedObject(target);
        SerializedProperty curSP = turtleSO.FindProperty("current");
        SerializedProperty defItersSP = turtleSO.FindProperty("defaultIterations");
        SerializedProperty astepSP = turtleSO.FindProperty("angleStep");
        SerializedProperty stepSP = turtleSO.FindProperty("step");
        SerializedProperty systemSP = turtleSO.FindProperty("system");
        SerializedProperty systemAxiomSP = systemSP.FindPropertyRelative("axiom");
        SerializedProperty systemProdsSP = systemSP.FindPropertyRelative("productions");

        GUI.enabled = false;
        EditorGUILayout.PropertyField(curSP, new GUIContent("Current String"));
        GUI.enabled = true;
        EditorGUILayout.PropertyField(defItersSP, new GUIContent("Iterations"));


        EditorGUILayout.PropertyField(astepSP, new GUIContent("Angle Step"));
        EditorGUILayout.PropertyField(stepSP, new GUIContent("Move Step"));
        EditorGUILayout.PropertyField(systemAxiomSP, new GUIContent("Axiom"));
        GUILayout.Label("Productions");
        for (int i = 0; i < systemProdsSP.arraySize; i++)
        {
            var p = systemProdsSP.GetArrayElementAtIndex(i);
            if (p.stringValue != "")
            {
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUILayout.TextField(((char)i).ToString(), GUILayout.Width(25));
                GUI.enabled = true;
                p.stringValue = EditorGUILayout.TextField(p.stringValue);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.BeginHorizontal();
        newProductionPred = EditorGUILayout.TextField("New ", newProductionPred, GUILayout.MinWidth(50));
        newProduction = EditorGUILayout.TextField(newProduction);
        if (GUILayout.Button("Add"))
        {
            var prod = systemProdsSP.GetArrayElementAtIndex((int)newProductionPred[0]);
            prod.stringValue = newProduction;
        }
        EditorGUILayout.EndHorizontal();

        turtleSO.ApplyModifiedProperties();

    }
}

[CustomEditor(typeof(PTurtle))]
public class PTurtleEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Find System")) {
            var turtle = target as PTurtle;
            var systemComp = turtle.GetComponent(typeof(IPLSystem)) as IPLSystem;
            if(systemComp != null) {
                turtle.system = systemComp.System;
                systemComp.InitializeSystem();
                if(turtle.system==null) Debug.LogError("Component does not have a parametric Lsystem!!");
                turtle.Path(turtle.defaultIterations);
            }
        }
    }
}

[CustomEditor(typeof(TurtleDrawer))]
public class TurtleDrawerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var drawer = target as TurtleDrawer;
        drawer.autoGenerate = GUILayout.Toggle(drawer.autoGenerate, "Automatically Regenerate");

        if(GUILayout.Button("Generate") || drawer.autoGenerate) {
            drawer.GeneratePath();
        }
    }
}

[CustomEditor(typeof(TurtleDrawer3D))]
public class TurtleDrawer3DEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var drawer = target as TurtleDrawer3D;
        drawer.autoGenerate = GUILayout.Toggle(drawer.autoGenerate, "Automatically Regenerate");

        if(GUILayout.Button("Generate") || drawer.autoGenerate) {
            drawer.GeneratePath();
            Debug.Log(drawer.turtle);
        }
    }
}
