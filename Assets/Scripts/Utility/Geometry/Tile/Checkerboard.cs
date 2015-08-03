using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geometry {
	public class Checkerboard : Tiling {

		public int width;
		public int height;

		public Checkerboard() {}

		public override string Name {
			get { return "Checkerboard"; }
		}

		public override Mesh Generate() {
			var tile = Shapes.Square().GenerateMesh();

			var verts = new List<Vector3>();
			var tris = new List<int>();
			var uvs = new List<Vector2>();
			var colors = new List<Color>();
			Vector3 offset = Vector3.zero;
			int triangleIndexBase = 0;
			for(int x = 0;x<width;x++) {
				for(int y = 0;y<height;y++) {
					verts.AddRange(tile.vertices.Select(v=>v+offset));
					tris.AddRange(tile.triangles.Select(i=>i+triangleIndexBase));
					uvs.AddRange(tile.uv);
					var color = (x+y)%2>0?Color.black:Color.white;
					colors.AddRange(Enumerable.Range(0,4).Select(i=>color));
					offset.y+=1;
					triangleIndexBase += tile.vertexCount;
				}
				offset.y=0;
				offset.x+=1;
			}
			var mesh = new Mesh();
			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.colors = colors.ToArray();
			return mesh;
		}
	}
}
