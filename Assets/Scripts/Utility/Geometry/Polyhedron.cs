using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace Geometry {
	public class Polyhedron {

        public Vertex[] vertices;
        public Edge[] edges;
        public Face[] faces;

		public class Vertex {
            public Vector3 position;
            public List<Edge> connected;
			public string Name;
			public Vertex(Vector3 pos) {
				position = pos;
                connected = new List<Edge>();
            }
        }

        public class Edge {
			public Vertex a, b;
            public List<Face> faces;
			public string Name;
			public bool ConnectedTo(Vertex v) {
				return v == a || v == b;
			}
			public bool ConnectedTo(Edge e) {
				return ConnectedTo(e.a) || ConnectedTo(e.b);
			}
            public Edge(Vertex a, Vertex b) {
                this.a = a; this.b = b;
                faces = new List<Face>();
				this.Name = a.Name + " --> " + b.Name;
            }
        }

		public class Face {
            public Edge[] edges;
			// string Name;
			public Face(params Edge[] edges) {
				this.edges = edges;
				// this.Name ="face: " + System.String.Join("\n", edges.Select(e=>e.Name).ToArray());
			}
			public IEnumerable<Vertex> OrderedVertices {
				get {
					var root = edges[0].a; // first vertex
					yield return root; // yield
					var found = new List<Vertex> { edges[0].b }; // collections of visited verts
					var nextVert = edges[0].b; // next vert to yield
					var next = edges[0]; // next edge to check
					while(true) {
						yield return nextVert; // yield
						next = edges.FirstOrDefault( // Get next edge
							e => !e.ConnectedTo(root) && // that isnt' connected to first vertex
								(found.Contains(e.a) ^ found.Contains(e.b)) // and has a vertex not returned yet
						);
						if(next!=null) { // return if found
							nextVert = found.Contains(next.a)?next.b:next.a;
							found.Add(nextVert); // add to visited collection
						} else {
							yield break;
						}
					}
				}
			}
        }


		public Polyhedron(Vector3[] vertices, int[][] edges, int[][] faces) {
            this.vertices = vertices.Select(v=>new Vertex(v)).ToArray();
			this.vertices.ForEach((v,i)=>v.Name = ""+i);
            this.edges = new Edge[edges.Length];
            for (int i = 0; i < edges.Length; i++)
            {
                this.edges[i] = new Edge(
					this.vertices[edges[i][0]],
					this.vertices[edges[i][1]]
				);
				this.vertices[edges[i][0]].connected.Add(this.edges[i]);
				this.vertices[edges[i][1]].connected.Add(this.edges[i]);
            }
            this.faces = new Face[faces.Length];
			for(int i=0;i<faces.Length;i++) {
                this.faces[i] = new Face(faces[i].Select(f=>this.edges[f]).ToArray());
				foreach(var e in this.faces[i].edges) {
					e.faces.Add(this.faces[i]);
				}
            }
        }

		public Mesh GenerateMesh() {

            var verts = new Vector3[faces.Sum(f=>f.edges.Length)];
            var tris = new int[faces.Sum(f => f.edges.Length - 2)*3];

            int fi = 0;
            int vi = 0;
            foreach(var face in faces) {
				var faceVerts = face.OrderedVertices;

				var root = faceVerts.First();
				var rootI = vi;
				verts[vi++] = root.position;

				var lastVert = faceVerts.Second();
				var lastI = vi;
				verts[vi++] = lastVert.position;

                foreach(var vert in faceVerts.Skip(2)) {
					verts[vi++] = vert.position;

					tris[fi++] = rootI;
					tris[fi++] = lastI;
					tris[fi++] = vi-1;

					lastVert = vert;
					lastI = vi-1;
                }
			}

            var mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;

            return mesh;
		}

		public void ColorVertices(Mesh m) {
			var verts = m.vertices;
			var colors = new Color[verts.Length];
			var minY = verts.Select(v=>v.y).Aggregate(0f, Mathf.Min);
			var maxY = verts.Select(v=>v.y).Aggregate(0f, Mathf.Max);
			for(int i=0;i<colors.Length;i++) {
				colors[i] = new ColorHSV(((float)i)/colors.Length,1f, Math.Place(minY, maxY, verts[i].y));
			}
			m.colors = colors;
		}
    }
}
