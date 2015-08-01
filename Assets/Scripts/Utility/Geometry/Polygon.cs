using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace Geometry {
	//TODO: Commence when this is a directed graph
	public class Polygon : DirectedGraph<Vector2>, IUnitTestable, IMeshGenerator {

		public Polygon()
		: base (
			new Vector2[]{
				Vector2.zero, Vector2.right, new Vector2(1,1), Vector2.up
			},
			new int[,]{{0,1},{1,2},{2,3},{3,0}}
		)
		{}

		public Polygon(Vector2[] vertices, int[,]edges)
		: base(vertices, edges) {}

		public string Name {
			get { return Vertices.Length+"-gon"; }
		}

		public bool IsSimple {
			get {
				return edges.Cross(edges)
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

		/** detects if an edge is a valid diagonal and does not intersect other edges */
		public bool Diagonal(Vertex<Vector2> a, Vertex<Vector2> b) {
			if(!Math2D.InCone(a.value,b.value,Last(a).value,Next(a).value) ||
			 	!Math2D.InCone(b.value,a.value,Last(b).value,Next(b).value)) {
				return false;
			}
			var last = DFS.First();
			foreach(var v in DFS.Skip(1)) {
				if(Math2D.LineLineIntersection(a.value,b.value,last,v)) return false;
				last = v;
			}
			return true;
		}

		/** Generates a graph with diagonals that define a minimally triangulated polygyon */
		public IEnumerable<int[]> Triangulate() {
			var verts = new LinkedList<int>(Enumerable.Range(0, Vertices.Length));
			var ears = Vertices.Select(
				v => Diagonal(Last(v), Next(v))
			).ToArray();
			var iters = 10000;
			while(verts.Count > 3 && iters-->0) {
				foreach(var vi in verts) {
					if(ears[vi]) {
						var v2 = verts.Find(vi);
						var v1 = v2.PreviousOrLast();
						var v0 = v1.PreviousOrLast();
						var v3 = v2.NextOrFirst();
						var v4 = v3.NextOrFirst();

						yield return new int[]{v1.Value, v2.Value, v3.Value};

						ears[v1.Value] = Diagonal(Vertices[v0.Value], Vertices[v3.Value]);
						ears[v3.Value] = Diagonal(Vertices[v1.Value], Vertices[v4.Value]);
						verts.Remove(v2);
						break;
					}
				}
			}
			if(iters == 0) {
				Debug.LogError("Exceeded maximum iterations!");
			} else {
				yield return verts.ToArray();
			}
		}

		public override void Test() {
			var square = new Polygon(
				new Vector2[]{
					Vector2.zero, Vector2.right, new Vector2(1,1), Vector2.up
				},
				new int[,]{{0,1},{1,2},{2,3},{3,0}}
			);
			square.Triangulate();
		}

		public Mesh Generate() {
			var verts = Vertices.Select(v=>(Vector3)v.value).ToArray();
			var tris = Triangulate().SelectMany(v=>v).ToArray();

			var mesh = new Mesh();
			mesh.vertices = verts;
			mesh.triangles = tris;
			return mesh;
		}
	}
}
