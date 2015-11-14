using UnityEngine;
using UnityEditor;
using System.Collections;
// FIXME: CharacterInfo assignmnt doesn't seem to work as of 5.2.2f1

public class FontEditorWindow : EditorWindow {

    Font currentFont;
    int currentChar;
    Rect currentRect;
    Texture fontTex;

    [MenuItem("Window/Font Editor")]
	public static void Open() {
		FontEditorWindow window = (FontEditorWindow)EditorWindow.GetWindow (typeof (FontEditorWindow));
		window.Init();
		window.Show();
	}

	void Init() {
		currentFont = null;
		fontTex = null;
	}

	void OnGUI() {
        var cf = (Font)EditorGUILayout.ObjectField("Font", currentFont, typeof(Font), false);
		if(cf!=currentFont) {
            currentFont = cf;
			fontTex = cf.material.mainTexture;
        }

        if (currentFont != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Preview:");
            GUILayout.BeginArea(GUILayoutUtility.GetRect(100, 100));
            GUI.DrawTextureWithTexCoords(
                GUILayoutUtility.GetRect(100, 100),
                fontTex,
                new Rect(	currentFont.characterInfo[currentChar].minX,
							currentFont.characterInfo[currentChar].maxY,
							currentFont.characterInfo[currentChar].glyphWidth,
							currentFont.characterInfo[currentChar].glyphHeight));
            GUILayout.EndArea();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            var mat = (Material)EditorGUILayout.ObjectField("Material", currentFont.material, typeof(Material), false);
            if (mat != currentFont.material)
            {
                currentFont.material = mat;
            }
            GUILayout.BeginHorizontal();
            for (int i=0;i<currentFont.characterInfo.Length;i++)
            {
                if (GUILayout.Button(((char)currentFont.characterInfo[i].index).ToString(), GUILayout.Width(25)))
                {
                    currentChar = i;
                    currentRect = new Rect(
                        currentFont.characterInfo[currentChar].minX,
                        -currentFont.characterInfo[currentChar].minY,
                        currentFont.characterInfo[currentChar].glyphWidth,
                        currentFont.characterInfo[currentChar].glyphHeight
                    );
                }
            }
            if (GUILayout.Button("New"))
            {
                var newinfo = new CharacterInfo[currentFont.characterInfo.Length + 1];
                System.Array.Copy(currentFont.characterInfo, newinfo, currentFont.characterInfo.Length);
                currentFont.characterInfo = newinfo;
                currentChar = currentFont.characterInfo.Length - 1;
            }
            GUILayout.EndHorizontal();
			bool changed = false;
            var ci = currentFont.characterInfo[currentChar];
            var ch = EditorGUILayout.TextField("Character", ((char)currentFont.characterInfo[currentChar].index).ToString());
            if ((int)ch[0] != ci.index)
            {
                ci.index = (int)ch[0];
                changed = true;
            }
            var x = (int)EditorGUILayout.Slider("X", currentRect.xMin, 0, fontTex.width);
            if (x != ci.minX)
            {
                currentRect.xMin = x;
                changed = true;
			}
            var y = (int)EditorGUILayout.Slider("Y", -ci.minY, 0, fontTex.height);
            if (y != -ci.minY)
            {
                ci.minY = -y;
				changed = true;
            }
            var w = (int)EditorGUILayout.Slider("W", ci.glyphWidth, 0, fontTex.width - ci.minX);
            if (w != ci.glyphWidth)
            {
                ci.glyphWidth = w;
				changed = true;
            }
            var h = (int)EditorGUILayout.Slider("H", ci.glyphHeight, 0, fontTex.height);
            if (h != ci.glyphHeight)
            {
                ci.glyphHeight = h;
				changed = true;
            }
			if(changed) {
                var nci = new CharacterInfo();
				nci.minX = (int)currentRect.xMin;
				nci.maxX = (int)currentRect.xMax;
				nci.minY = (int)currentRect.yMax;
				nci.maxY = (int)currentRect.yMin;
				nci.index = ci.index;
                currentFont.characterInfo[currentChar] = nci;
				Debug.Log(currentFont.characterInfo[currentChar].minX);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
