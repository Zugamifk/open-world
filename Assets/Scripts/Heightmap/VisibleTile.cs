using UnityEngine;
using System.Collections;

namespace Landscape {
	[RequireComponent(typeof(MeshFilter))]
	public class VisibleTile : MonoBehaviour {

		public Tile tile;

		private MeshFilter meshFilter;
		private const int worldObjectPoolCap = 5;
		private WorldObjecInitializer[] plantPool;
		private int currentObjects = 0;

		private void ClearPools() {
			for(int i=0;i<currentObjects;i++) {
				plantPool[i].Clear();
			}
			currentObjects = 0;
		}

		public void Init() {
			gameObject.AddComponent<MeshRenderer>();
			plantPool = new WorldObjecInitializer[worldObjectPoolCap];
			for(int i=0;i<worldObjectPoolCap;i++) {
				var go = new GameObject();
				go.transform.OrientTo(transform);
				go.name = "Plant";
				var woi = go.AddComponent<WorldObjecInitializer>();
				plantPool[i] = woi;
			}
			currentObjects = 0;
		}

		public void Refresh() {
			gameObject.name = tile.Name;
			meshFilter.mesh = tile.Mesh;
			ClearPools();
			foreach(var obj in tile.worldObjects) {
				plantPool[currentObjects].Initialize(obj);
				currentObjects++;
			}
		}

		void Awake() {
			meshFilter = GetComponent<MeshFilter>();
		}
	}
}
