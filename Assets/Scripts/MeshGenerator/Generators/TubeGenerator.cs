﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MeshGenerator;

public class TubeGenerator : IMeshGenerator {

    public int sides;

    private Spline path;
    public struct TubePoint {
        public Vector3 position;
        public float radius;
    }
    private List<TubePoint> points = new List<TubePoint>();

	public void AddPoint(Vector3 position, float radius) {
		points.Add(new TubePoint{position = position, radius = radius});
	}

    public string Name {
        get { return "Tube Generator";  }
    }

	public Mesh Generate() {
        path = new Spline();
		foreach(var point in points) {
			path.Add(point.position);
		}

		var Verts = new Vector3[sides*points.Count];
		int vi = 0;
		var angle = Mathf.PI*2f/(float)sides;
		var sideAngle = (Mathf.PI-angle)/2f;
        for (int s = 0; s < points.Count; s++)
        {
            foreach (var v in Utils.GetRingPoints(sides, path.EvaluateDerivative((float)s), new Vector3(points[s].radius, 0, 0)))
            {
                Verts[vi] = v;
                vi++;
            }
        }

        // Build faces, duplicating vertices for each face so we get hard edges
        var verts = new Vector3[sides*points.Count*4];
		var tris = new int[verts.Length*6];
		System.Func<int, int> wrap = i => (i % sides);
		for (int i = 0; i < sides; i++)
		{
			Utils.DuplicateVerts(Verts, verts, i * 4, i, i + sides, wrap(i + 1) + sides, wrap(i + 1));
			Utils.AddFace(tris, i * 6, i * 4, 4);
		}


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
