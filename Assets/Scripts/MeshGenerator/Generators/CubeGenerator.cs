using UnityEngine;
using System;
using System.Collections;
using Geometry;

namespace MeshGenerator
{
    public class Cube : IMeshGenerator
    {

        public float sideLength = 1;

        public string Name
        {
            get { return "Cube"; }
        }

        public Mesh Generate()
        {

            // build basic mesh with two 4-vertex rings
            var cubeVerts = new Vector3[8];
            int vi = 0;
            foreach (var v in Utils.GetRingPoints(4, Vector3.up, new Vector3(-0.5f, 0, -0.5f)))
            {
                cubeVerts[vi] = (v + Vector3.up * 0.5f) * sideLength;
                cubeVerts[vi + 4] = (v + Vector3.down * 0.5f) * sideLength;
                vi++;
            }

            // Build faces, duplicating vertices for each face so we get hard edges
            var verts = new Vector3[24];
            var tris = new int[36];
            Func<int, int> wrap = i => (i % 4);
            for (int i = 0; i < 4; i++)
            {
                Utils.DuplicateVerts(cubeVerts, verts, i * 4, i, i + 4, wrap(i + 1) + 4, wrap(i + 1));
                Utils.AddFace(tris, i * 6, i * 4, 4);
            }
            Utils.DuplicateVerts(cubeVerts, verts, 16, 0, 1, 2, 3);
            Utils.AddFace(tris, 24, 16, 4);
            Utils.DuplicateVerts(cubeVerts, verts, 20, 7, 6, 5, 4);
            Utils.AddFace(tris, 30, 20, 4);

            // UVs mapping sets each face to use the whole texture
            var uv = new Vector2[24];
            var corners = new Vector2[]
            {
                Vector2.zero,
                Vector2.up,
                new Vector2(1,1),
                Vector2.right
            };
            for (int face = 0; face < 6; face++)
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
