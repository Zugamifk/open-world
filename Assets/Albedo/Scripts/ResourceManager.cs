using UnityEngine;
using System.Collections;

namespace Albedo {
	public class ResourceManager : MonoBehaviour {

        [SerializeField] protected Defaults m_defaults;

		public static Defaults Defaults {
			get {
                return instance.m_defaults;
            }
		}

        private static ResourceManager instance;

        void Awake() {
            this.SetInstanceOrKill(ref instance);
        }
	}
}
