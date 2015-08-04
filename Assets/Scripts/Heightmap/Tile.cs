using UnityEngine;
using System.Collections;
using MeshGenerator;
using Geometry;
using Extensions;
using System.Linq;

namespace Landscape {
	public class Tile : MonoBehaviour, IMeshGenerator {

		public Vector2 position;
		public IHeightMap map;
		public Tiling tilingMeshGenerator;
		public int gridWidth, gridHeight;

		private Mesh mesh;
		private MeshFilter filter;
		public Mesh Mesh {
			get { return mesh; }
			set {
				mesh = value;
				if(filter != null) filter.mesh = value;
			}
		}

		const string _tileNamePrefix = "tile ";
		public string Name {
			get {
				return _tileNamePrefix + position;
			}
		}

		public Mesh Generate() {

			var mesh = tilingMeshGenerator.Generate();
			var verts = mesh.vertices;

			float ws = 1/(float)gridWidth;
			float hs = 1/(float)gridHeight;

			for(int i=0;i<verts.Length;i++) {
				verts[i] = verts[i].x0y();
				verts[i].x*= ws;
				verts[i].z*= hs;
				verts[i].y = map.GetHeight(position.x+verts[i].x, position.y+verts[i].z);
			}

			mesh.vertices = verts;

			Utils.PostGenerateMesh(mesh);

			var filter = GetComponent<MeshFilter>();
			if(filter!=null) filter.mesh = mesh;

			return mesh;
		}

		void Start() {
			filter = GetComponent<MeshFilter>();
		}
	}
}
