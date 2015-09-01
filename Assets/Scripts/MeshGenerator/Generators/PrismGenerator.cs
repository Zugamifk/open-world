using UnityEngine;
using System.Collections;
using System;

namespace MeshGenerator
{
	public class PrismGenerator : IMeshGenerator {

        public int sides;
        public float length;
        public float sideLength;

        public string Name {
            get { return "Prism"; }
        }

		public Mesh Generate() {
			// build basic mesh with two 4-vertex rings
            var Verts = new Vector3[sides*2];
            int vi = 0;
			var angle = Mathf.PI*2f/(float)sides;
			var sideAngle = (Mathf.PI-angle)/2f;
            var radius = sideLength * Mathf.Sin(sideAngle) / Mathf.Sin(angle);
            foreach (var v in Utils.GetRingPoints(sides, Vector3.up, new Vector3(radius,0,0)))
            {
                Verts[vi] = (v + Vector3.up * 0.5f * length);
                Verts[vi + sides] = (v + Vector3.down * 0.5f * length);
                vi++;
            }

            // Build faces, duplicating vertices for each face so we get hard edges
            var verts = new Vector3[sides*6];
            var tris = new int[sides*6 + (sides-2)*6];
            Func<int, int> wrap = i => (i % sides);
            for (int i = 0; i < sides; i++)
            {
                Utils.DuplicateVerts(Verts, verts, i * 4, i, i + sides, wrap(i + 1) + sides, wrap(i + 1));
                Utils.AddFace(tris, i * 6, i * 4, 4);
            }
            Utils.DuplicateVerts(Verts, verts, sides*4, 0, sides);
            Utils.AddFace(tris, sides*6, sides*4, sides);
            Utils.DuplicateVerts(Verts, verts, sides*5, sides, -sides);
            Utils.AddFace(tris, sides*6+(sides-2)*3, sides*5, sides);

            // UVs mapping sets each face to use the whole texture
            var uv = new Vector2[verts.Length];
            var corners = new Vector2[]
            {
                Vector2.zero,
                Vector2.up,
                new Vector2(1,1),
                Vector2.right
            };
            for (int face = 0; face < sides; face++)
            {
                for (int c = 0; c < 4; c++)
                {
                    uv[face * 4 + c] = corners[c];
                }
            }

            var mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;

            return mesh;
		}
	}
}
