using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Audio {
	[CustomEditor(typeof(Player))]
	public class PlayerEditor : Editor {

		private ClassSelector<IAudioGenerator> selector;
		private List<ClassSelector<IAudioFilter>> filterSelectors = new List<ClassSelector<IAudioFilter>>();

		void OnEnable() {
			selector = new ClassSelector<IAudioGenerator>();
			filterSelectors = new List<ClassSelector<IAudioFilter>>();
		}

		void RefreshFilters() {
			var player = target as Player;
			player.filters = filterSelectors.Select(s=>s.Instance).ToList();
		}

		bool filtersOpen = false;
		public override void OnInspectorGUI() {
			var player = target as Player;
			DrawDefaultInspector();
			EditorGUILayout.Space();

			if(selector!=null)
				player.generator = selector.DrawField(player.generator);
			EditorGUILayout.Space();
			filtersOpen = EditorGUILayout.Foldout(filtersOpen, "Filters");
			if(filtersOpen) {
				for(int i=0;i<player.filters.Count;i++) {
					player.filters[i] = filterSelectors[i].DrawField(player.filters[i]);
					EditorGUILayout.Space();
				}
				if(GUILayout.Button("New")) {
					var newSelector = new ClassSelector<IAudioFilter>();
					filterSelectors.Add(newSelector);
					newSelector.DeleteEvent += () => {
						filterSelectors.Remove(newSelector);
						RefreshFilters();
					};
					player.filters.Add(newSelector.Instance);
				}
			}

			if(GUILayout.Button("Play"))
				player.Play();
		}
	}
}
