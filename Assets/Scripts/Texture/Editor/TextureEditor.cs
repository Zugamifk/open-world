using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;

namespace TextureGenerator {
	public class TextureEditor : EditorWindow {

        const string kTexturePath = "Assets/Textures/";

		Texture2D CurrentTextureAsset;
		TextureGenerator CurrentGenerator;

        ClassSelector<TextureGenerator> selector;

        [MenuItem("Window/Texture Generator")]
		public static void Open() {
			TextureEditor window = (TextureEditor)EditorWindow.GetWindow (typeof (TextureEditor));
			window.Init();
			window.Show();
		}

		void Init() {

            selector = new ClassSelector<TextureGenerator>();

            CurrentGenerator = selector.Instance;

        }

		void OnEnable() {
			Init();
            if (CurrentTextureAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(CurrentTextureAsset);
                TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);

                ti.textureType = TextureImporterType.Sprite;
                ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				ti.mipmapEnabled = false;
				ti.isReadable = true;

                AssetDatabase.ImportAsset(path);
            }
		}

        Vector2 scrollPos;
        void OnGUI() {
            CurrentGenerator = selector.DrawField(CurrentGenerator);

			var ta = (Texture2D)EditorGUILayout.ObjectField("Texture File:", CurrentTextureAsset, typeof(Texture2D), false, GUILayout.Height(36));
			if (ta!=CurrentTextureAsset) {
                if (ta != null)
                {
                    string path = AssetDatabase.GetAssetPath(ta);
                    TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);

                    ti.textureType = TextureImporterType.Sprite;
                    ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
					ti.mipmapEnabled = false;
					ti.isReadable = true;

                    AssetDatabase.ImportAsset(path);
                }
                CurrentTextureAsset = ta;
            }

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Regenerate Texture")) {
				RegenerateTexture();
			}

			if(GUILayout.Button("Save Asset")) {
                SaveTexture();
            }
			GUILayout.EndHorizontal();

            if (CurrentTextureAsset != null)
            {
				GUILayout.Label("Preview:");
                EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(256, 256), CurrentTextureAsset, null, ScaleMode.ScaleToFit);
            }
        }

		void RegenerateTexture() {

			Color[] pixels = CurrentGenerator.GetPixels().ToArray();

			// Update/save texture asset
			if(CurrentTextureAsset!=null) {
				CurrentTextureAsset.Resize(CurrentGenerator.width, CurrentGenerator.height);
                CurrentTextureAsset.SetPixels(pixels);
				CurrentTextureAsset.Apply();

			} else {
                Texture2D texture = CurrentGenerator.Generate();
                AssetDatabase.CreateAsset(texture, kTexturePath+"New Texture.asset");
				CurrentTextureAsset = texture;
			}
			Debug.Log("Generated texture "+CurrentTextureAsset.name+" with "+CurrentGenerator.Name);
		}

		void SaveTexture() {
			File.WriteAllBytes(kTexturePath+CurrentTextureAsset.name+".jpg", CurrentTextureAsset.EncodeToJPG());
			EditorUtility.SetDirty(CurrentTextureAsset);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
        }
	}
}
