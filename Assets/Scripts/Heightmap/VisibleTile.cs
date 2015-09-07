using UnityEngine;
using System.Collections;

namespace Landscape {
	[RequireComponent(typeof(MeshFilter))]
	public class VisibleTile : MonoBehaviour {

		public Tile tile;

		private MeshFilter meshFilter;
		private const int transformPoolCap = 100;
		private Transform[] plantPool;

		public void Init() {
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>();
			plantPool = new Transform[transformPoolCap];
			for(int i=0;i<transformPoolCap;i++) {
				var tf = new GameObject();
				tf.OrientTo(transform);
				plantPool[i] = tf.transform;
			}
		}

		public void Refresh() {
			gameObject.name = tile.Name;
			meshFilter.mesh = tile.Mesh;
		}

		void Awake() {
			meshFilter = GetComponent<MeshFilter>();
		}
	}
}
