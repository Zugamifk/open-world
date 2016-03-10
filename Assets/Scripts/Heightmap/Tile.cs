using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MeshGenerator;
using Geometry;
using Extensions;
using System.Linq;

namespace Landscape {
	public class Tile : IMeshGenerator {

		public Vector2 position;
		public IHeightMap map;
		public IMeshGenerator meshGenerator;
		public List<WorldObject> worldObjects = new List<WorldObject>();

		private Mesh mesh;
		public Mesh Mesh {
			get { return mesh; }
			set { mesh = value; }
		}

		const string _tileNamePrefix = "tile ";
		public string Name {
			get {
				return _tileNamePrefix + position;
			}
		}

		public Mesh Generate() {

			var mesh = meshGenerator.Generate();

			mesh.RecalculateBounds();
			mesh.Normalize();

			var verts = mesh.vertices;

			for(int i=0;i<verts.Length;i++) {
				verts[i] = ((Vector2)verts[i]).x0y();
				verts[i].y = map.GetHeight(position.x+verts[i].x, position.y+verts[i].z);
			}

			mesh.vertices = verts;

			Utils.PostGenerateMesh(mesh);

			return mesh;
		}
	}
}
