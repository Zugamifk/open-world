﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using MeshGenerator;

namespace Geometry {
	public class Polygon : DirectedGraph<Vector2>, IUnitTestable {

		public Polygon(params Vector2[] vertices) {
			Vertices = vertices.Select(v=>new Vertex<Vector2>(v)).ToList();
			Edges = new List<Edge<Vector2>>();
			int N = Vertices.Count;
			for(int i=0;i<N;i++) {
				AddEdge(Vertices[i], Vertices[(i+1)%N]);
			}
		}
		
		public string Name {
			get { return Vertices.Count+"-gon"; }
		}

		public bool IsConvex {
			get {
				return Vertices.All(
					v => Diagonal(Last(v), Next(v))
				);
			}
		}

		public bool IsSimple {
			get {
				return Edges.Cross(Edges)
					.Where(pair=>!pair.First().ConnectedTo(pair.Second()))
					.All(pair=>!Math3D.LineLineIntersection(
						pair.First().from.value, pair.First().to.value,
						pair.Second().from.value, pair.Second().to.value
					));
			}
		}

		public Vertex<Vector2> Next(Vertex<Vector2> v) {
			var n = v.edges.FirstOrDefault(e=>e.from == v);
			if(n==null) Debug.LogWarning("Vertex "+v+" has no predecessor!");
			return n==null?null:n.to;
		}
		public Vertex<Vector2> Last(Vertex<Vector2> v) {
			var n = v.edges.FirstOrDefault(e=>e.to == v);
			if(n==null) Debug.LogWarning("Vertex "+v+" has no successor!");
	  		return n==null?null:n.from;
		}

		public float Angle(Vertex<Vector2> v) {
			return Vector2.Angle(v.value-Last(v).value, Next(v).value-v.value);
		}

		/** detects if an edge is a valid diagonal and does not intersect other edges */
		public bool Diagonal(Vertex<Vector2> a, Vertex<Vector2> b, IEnumerable<int> submesh = null) {
			if(!Math2D.InCone(a.value,b.value,Last(a).value,Next(a).value) ||
			 	!Math2D.InCone(b.value,a.value,Last(b).value,Next(b).value)) {
				return false;
			}
			submesh = submesh ?? Enumerable.Range(0, Vertices.Count);
			var last = submesh.First();
			foreach(var v in submesh.Skip(1)) {
				if(Math2D.LineLineIntersection(a.value,b.value,Vertices[last].value,Vertices[v].value)) return false;
				last = v;
			}
			return true;
		}

		/** Generates a graph with diagonals that define a minimally triangulated polygyon */
		public IEnumerable<int[]> Triangulate() {
			var verts = new LinkedList<int>(Enumerable.Range(0, Vertices.Count));
			var ears = Vertices.Select(
				v => Diagonal(Last(v), Next(v))
			).ToArray();
			var iters = 10000;
			bool hasEars = true;
			while(verts.Count > 3 && iters-->0 && hasEars) {
				hasEars = false;
				Debug.Log(verts.Count);
				foreach(var vi in verts) {
					if(ears[vi]) {
						hasEars = true;
						var v2 = verts.Find(vi);
						var v1 = v2.PreviousOrLast();
						var v0 = v1.PreviousOrLast();
						var v3 = v2.NextOrFirst();
						var v4 = v3.NextOrFirst();

						yield return new int[]{v1.Value, v2.Value, v3.Value};

						if(!verts.Remove(vi)) Debug.LogError("Removing "+vi+" failed!");
						ears[v1.Value] = Diagonal(Vertices[v0.Value], Vertices[v3.Value], verts);
						ears[v3.Value] = Diagonal(Vertices[v1.Value], Vertices[v4.Value], verts);
						break;
					}
				}
			}
			if(!hasEars) {
				Debug.LogError("Couldn't Find a valid triangle to generate! Unmatched vertices: "+verts.Count);
			} else
			if(iters < 0) {
				Debug.LogError("Exceeded maximum iterations!");
			} else if (verts.Count!=3) {
				Debug.LogError("Did not triangulate properly! Degenerate vertex count: "+verts.Count);
			} else {
				yield return verts.ToArray();
			}
		}


		public Mesh GenerateMesh() {
			var verts = Vertices.Select(v=>(Vector3)v.value).ToArray();
			var tris = Triangulate().SelectMany(v=>v).ToArray();
			Debug.Log("Triangles: "+tris.Length/3);
			Debug.Log("Vertices: "+Vertices.Count);
			Utils.FlipTriangles(tris);
			var bounds = Rectx.BoundingRect(Vertices.Select(v=>v.value).ToArray());
			var uvs = verts.Select(v=>Rect.PointToNormalized(bounds, v)).ToArray();

			var mesh = new Mesh();
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uvs;
			return mesh;
		}

		private const int maxGizmos = 256;
		public void DrawGizmos(Color c, Vector3 position) {
			Gizmos.color = c;
			for(int i=0;i<Mathf.Min(Vertices.Count, maxGizmos);i++) {
				Gizmos.DrawSphere(position+(Vector3)Vertices[i].value, 0.2f);
				Gizmos.DrawLine(position+(Vector3)Vertices[i].value, position+(Vector3)Vertices[(i+1)%Vertices.Count].value);
			}
		}

		public override void Test() {
			var square = new Polygon(
				new Vector2[]{
					Vector2.zero, Vector2.right, new Vector2(1,1), Vector2.up
				}
			);
			square.Triangulate();
		}
	}

	public class PolygonGenerator : IMeshGenerator {

		public enum Shape {
			NONE,
			NGON,
			KITE
		}

		public Polygon polygon;
		public Shape shape;

		public PolygonGenerator() {
			polygon = Shapes.Square();
		}

		public string Name {
			get { return shape.ToString().Uncamel(); }
		}

		public Mesh Generate() {

			switch(shape) {
				case Shape.NONE: polygon = Shapes.Monogon(); break;
				case Shape.NGON: polygon = Shapes.Square(); break;
				case Shape.KITE: polygon = Shapes.Kite(); break;
			}

			return polygon.GenerateMesh();
		}
	}
}
