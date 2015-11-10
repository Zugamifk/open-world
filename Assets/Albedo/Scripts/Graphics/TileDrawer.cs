using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {
	public class TileDrawer : MonoBehaviour {

		public static Transform Root {
			get {
				return instance.transform;
			}
		}

        private static TileDrawer instance;
        void Awake() {
            this.SetInstanceOrKill(ref instance);
        }
	}
}
