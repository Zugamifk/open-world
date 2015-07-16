using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Audio {
	[CustomEditor(typeof(Player))]
	public class PlayerEditor : Editor {

		private ClassSelector<IAudioGenerator> selector;
		private IAudioGenerator generator;

		void OnEnable() {
			selector = new ClassSelector<IAudioGenerator>();
			Debug.Log("ho!");
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if(selector!=null)
				generator = selector.DrawField(generator);
		}
	}
}
