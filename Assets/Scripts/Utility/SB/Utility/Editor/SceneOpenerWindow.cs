using UnityEngine;
using UnityEditor;
using SilverBuild;
using System.IO;
using System.Linq;

public class SceneOpenerWindow : EditorWindow {
	
	protected const string kFocusName = "SceneOpenerWindow.SceneFilterControl";
	
	protected string m_searchText = "";
	
	protected Vector2 m_scrollPosition;
	
	protected string m_selected = null;
	protected int m_selectedIndex = 0;
	
	//Command to open the editor window
	[MenuItem("File/Open Scene Quickly... %t")]
	protected static void OpenWindow() {
		GetWindow(
			typeof(SceneOpenerWindow),
			true,
			"Open Scene..."
		);
	}
	
	protected void OnLostFocus() {
		SafeClose();
	}
	
	protected void OnGUI() {
		if (Event.current.type == EventType.KeyDown) {
			switch (Event.current.keyCode) {
				case KeyCode.Escape:
					Event.current.Use();
					SafeClose();
					break;
				case KeyCode.UpArrow:
					Event.current.Use();
					--m_selectedIndex;
					m_selectedIndex = Mathf.Max(0, m_selectedIndex);
					break;
				case KeyCode.DownArrow:
					Event.current.Use();
					++m_selectedIndex;
					break;
				case KeyCode.KeypadEnter:
				case KeyCode.Return:
					Event.current.Use();
					if (m_selected != null) {
						CloseWindowAndOpenScene(m_selected);
					}
					break;
			}
		}
		
		GUI.SetNextControlName(kFocusName);
		string newSearchText = EditorGUILayout.TextField(m_searchText);
		bool textChanged = newSearchText != m_searchText;
		m_searchText = newSearchText;
		EditorGUI.FocusTextInControl(kFocusName);
		
		string[] tokenizedSearch = m_searchText.ToLower().Split(' ');
		
		int resultIndex = 0;
		
		m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
			foreach (
				string scenePath in
					from scenePath in SceneInfo.ScenePaths
					where tokenizedSearch.All(
						x => Path.GetFileNameWithoutExtension(scenePath)
								.ToLower().Contains(x))
					select scenePath
			) {
				
				if (textChanged && resultIndex == 0) {
					m_selected = scenePath;
					m_selectedIndex = resultIndex;
				} else if (resultIndex <= m_selectedIndex) {
					m_selected = scenePath;
				}
				
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);
				
				Rect elementRect = GUILayoutUtility.GetRect(
					new GUIContent(sceneName),
					EditorStyles.boldLabel
				);
				
				bool hover = elementRect.Contains(Event.current.mousePosition);
				
				if (Event.current.type == EventType.repaint) {
					EditorStyles.boldLabel.Draw(
						elementRect,
						sceneName,
						hover,
						false,
						resultIndex == m_selectedIndex,
						false
					);
				}
				
				if (hover && Event.current.type == EventType.MouseDown) {
					Event.current.Use();
					CloseWindowAndOpenScene(scenePath);
				}
				++resultIndex;
			}
		EditorGUILayout.EndScrollView();
		if (resultIndex == 0) {
			GUILayout.Label("No results");
			GUILayout.FlexibleSpace();
			m_selected = null;
		} else if (m_selectedIndex == resultIndex) {
			m_selectedIndex = resultIndex - 1;
		}
	}
	
	protected void CloseWindowAndOpenScene(string scenePath) {
		if (scenePath == null) {
			Debug.LogError("SceneOpener told to open null scene! Ignoring.");
			return;
		}
		SafeClose();
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
			EditorApplication.OpenScene(scenePath);
		}
	}
	
	protected void SafeClose() {
		EditorApplication.delayCall += Close;
	}
}