using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shrines {
    [CustomEditor(typeof(TileGraphicData))]
    public class TileGraphicDataInspector : Editor {

        SerializedProperty graphicsProp;
        
        [SerializeField]
        List<bool> tileDropdowns = new List<bool>();
        [SerializeField]
        bool showDepthLayers;
        [SerializeField]
        bool showDefaultSprites;

        string[] surfaceNames;

        const int previewSize = 75;
        const int optionSize = 15;
        const int sep = 5;

        void OnEnable()
        {
            graphicsProp = serializedObject.FindProperty("graphics");
            if (tileDropdowns.Count < graphicsProp.arraySize)
            {
                tileDropdowns.AddRange(Linqx.Generate(() => false).Take(graphicsProp.arraySize - tileDropdowns.Count));
            }

            surfaceNames = System.Enum.GetNames(typeof(Tile.Surface));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Surfaces", EditorStyles.boldLabel);
            for (int i = 0; i < graphicsProp.arraySize; i++)
            {
                var graphic = graphicsProp.GetArrayElementAtIndex(i);
                var surf = graphic.FindPropertyRelative("surface");

                tileDropdowns[i] = EditorGUILayout.BeginToggleGroup(surfaceNames[surf.enumValueIndex], tileDropdowns[i]);

                if (tileDropdowns[i])
                {
                    var layers = graphic.FindPropertyRelative("sprites");

                    DrawSpriteLayers(layers);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(surf);
                    if (GUILayout.Button("Add Layer"))
                    {
                        layers.InsertArrayElementAtIndex(layers.arraySize);
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete surface"))
                    {
                        graphicsProp.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        i--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
                EditorGUILayout.EndToggleGroup();
            }

            if (GUILayout.Button("Add New"))
            {
                graphicsProp.InsertArrayElementAtIndex(graphicsProp.arraySize);
                tileDropdowns.Add(false);   
            }

            GUILayout.Space(20);

            showDepthLayers = EditorGUILayout.BeginToggleGroup("Depth Layers", showDepthLayers);
            if (showDepthLayers)
            {
                var depths = serializedObject.FindProperty("depthlayers");
                int lastDepth = 0;
                for (int d = 0; d < depths.arraySize; d++)
                {
                    var depth = depths.GetArrayElementAtIndex(d);
                    var depthVal = depth.FindPropertyRelative("depth");
                    EditorGUILayout.LabelField(lastDepth + "-" + depthVal.intValue, EditorStyles.centeredGreyMiniLabel);
                    lastDepth = depthVal.intValue + 1;
                    var layer = depth.FindPropertyRelative("sprites");

                    DrawSpriteLayer(layer);
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PropertyField(depthVal);
                    if (GUILayout.Button("Delete Layer"))
                    {
                        depths.DeleteArrayElementAtIndex(d);
                        serializedObject.ApplyModifiedProperties();
                        d   --;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();   

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(layer.FindPropertyRelative("layer"));
                    if (GUILayout.Button("Add Shape"))
                    {
                        var shapes = layer.FindPropertyRelative("shapes");
                        shapes.InsertArrayElementAtIndex(shapes.arraySize);
                    }
                    EditorGUILayout.EndHorizontal();   
                }
                if (GUILayout.Button("Add Layer"))
                {
                    depths.InsertArrayElementAtIndex(depths.arraySize);
                }
                GUILayout.Space(20);

            }
            EditorGUILayout.EndToggleGroup();


            showDefaultSprites = EditorGUILayout.BeginToggleGroup("Default Sprites", showDefaultSprites);
            if (showDefaultSprites)
            {
                var defaultSprites = serializedObject.FindProperty("defaultSprites");
                DrawSpriteLayers(defaultSprites);
                if (GUILayout.Button("Add Layer"))
                {
                    defaultSprites.InsertArrayElementAtIndex(defaultSprites.arraySize);
                }
                GUILayout.Space(20);
            }
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTile"));

            serializedObject.ApplyModifiedProperties(); 
        }

        void DrawSpriteLayers(SerializedProperty layers)
        {
            for (int l = 0; l < layers.arraySize; l++)
            {
                EditorGUI.indentLevel++;

                var layer = layers.GetArrayElementAtIndex(l);
                DrawSpriteLayer(layer);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(layer.FindPropertyRelative("layer"));
                if (GUILayout.Button("Add Shape"))
                {
                    var shapes = layer.FindPropertyRelative("shapes");
                    shapes.InsertArrayElementAtIndex(shapes.arraySize);
                }
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Delete Layer"))
                {
                    layers.DeleteArrayElementAtIndex(l);
                    serializedObject.ApplyModifiedProperties();
                    l--;
                    continue;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        void DrawSpriteLayer(SerializedProperty layer)
        {
            var shapes = layer.FindPropertyRelative("shapes");
            var defaultSprite = layer.FindPropertyRelative("defaultShape");
            for (int s = 0; s < shapes.arraySize; s++)
            {
                var contextRect = EditorGUILayout.BeginVertical();

                EditorGUI.indentLevel++;
                var shape = shapes.GetArrayElementAtIndex(s);
                var tiles = shape.FindPropertyRelative("sprites");
                var step = previewSize + optionSize + sep;
                var perRow = (int)(Screen.width / step);
                var rows = tiles.arraySize / perRow + 1;
                int t = 0;

                for (int r = 0; r < rows; r++)
                {
                    var rect = EditorGUILayout.GetControlRect(true, previewSize);
                    var texRect = rect;
                    texRect.width = previewSize - 5;
                    texRect.height = previewSize - 5;
                    texRect.position = rect.position + Vector2.one * 2;

                    int ti = 0;
                    for (; ti < perRow && t < tiles.arraySize; ti++)
                    {
                        var texprop = tiles.GetArrayElementAtIndex(t);
                        texprop.objectReferenceValue = EditorGUI.ObjectField(texRect, texprop.objectReferenceValue, typeof(Sprite));
                        var br = texRect;
                        br.size = new Vector2(optionSize, optionSize);
                        br.position += new Vector2(texRect.width + sep, 0);
                        if (GUI.Button(br, "x"))
                        {
                            tiles.DeleteArrayElementAtIndex(t);
                            serializedObject.ApplyModifiedProperties();
                            t--;
                        }
                        texRect.position += Vector2.right * step;
                        t++;
                    }

                    if (t == tiles.arraySize && ti < perRow)
                    {
                        if (GUI.Button(texRect, new GUIContent("+")))
                        {
                            tiles.InsertArrayElementAtIndex(tiles.arraySize);
                        }
                    }
                }

                var dims = shape.FindPropertyRelative("dimensions");
                EditorGUILayout.PropertyField(dims);
                var x = dims.FindPropertyRelative("x");
                var y = dims.FindPropertyRelative("y");
                if (x.intValue < 1)
                {
                    x.intValue = 1;
                }

                if (y.intValue < 1)
                {
                    y.intValue = 1;
                }

                EditorGUILayout.PropertyField(shape.FindPropertyRelative("offset"));

                var shapeStep = shape.FindPropertyRelative("step");
                EditorGUILayout.PropertyField(shapeStep);
                x = shapeStep.FindPropertyRelative("x");
                y = shapeStep.FindPropertyRelative("y");
                if (x.intValue < 1)
                {
                    x.intValue = 1;
                }

                if (y.intValue < 1)
                {
                    y.intValue = 1;
                }

                EditorGUILayout.BeginHorizontal();

                var isDefault = defaultSprite.intValue == s;
                var nv = EditorGUILayout.Toggle("Default", isDefault);
                if (nv != isDefault)
                {
                    defaultSprite.intValue = s;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete Shape"))
                {
                    shapes.DeleteArrayElementAtIndex(s);
                    serializedObject.ApplyModifiedProperties();
                    s--;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
                DoShapeContextMenu(contextRect, shape, s, shapes);
            }
        }

        void DoShapeContextMenu(Rect rect, SerializedProperty prop, int arrayIndex, SerializedProperty arrayProp)
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.ContextClick)
            {
                Vector2 mousePos = currentEvent.mousePosition;
                if (rect.Contains(mousePos))
                {
                    // Now create the menu, add items and show it
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Move Up"), arrayIndex > 0, () => MoveElement(arrayProp, arrayIndex, -1));
                    menu.AddItem(new GUIContent("Move Down"), arrayIndex < arrayProp.arraySize - 1, () => MoveElement(arrayProp, arrayIndex, 1));
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Shape"), true, ()=>DeleteElement(arrayProp, arrayIndex));
                    menu.ShowAsContext();
                    currentEvent.Use();
                }
            }
        }

        void MoveElement(SerializedProperty prop, int index, int step)
        {
            Debug.Log("Move element " + index + " of " + prop.displayName + " to " + (index + step));
            prop.MoveArrayElement(index, index + step);
            serializedObject.ApplyModifiedProperties();
        }

        void DeleteElement(SerializedProperty array, int index)
        {
            array.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

    }
}