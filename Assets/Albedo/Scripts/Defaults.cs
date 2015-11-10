using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Albedo {
	public class Defaults : ScriptableObject
	{
        public Sprite tileSprite;

		#if UNITY_EDITOR
		[MenuItem("Assets/Create/Defaults")]
		public static void CreateAsset ()
		{
			ScriptableObjectUtility.CreateAsset<Defaults> ();
		}
		#endif
    }
}
