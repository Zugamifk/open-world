using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geometry {
	public class Polyhedron {

        public Vertex[] vertices;
        public Edge[] edges;
        public Face[] faces;

		public class Vertex {
            public Vector3 position;
            public List<Edge> connected;
			public Vertex(Vector3 pos) {
				position = pos;
                connected = new List<Edge>();
            }
        }

        public class Edge {
			public Vertex a, b;
            public List<Face> faces;
            public Edge(Vertex a, Vertex b) {
                this.a = a; this.b = b;
                faces = new List<Face>();
            }
        }

		public class Face {
            public Edge[] edges;
			public Face(params Edge[] edges) {
				this.edges = edges;
			}
        }

		public Polyhedron(Vector3[] vertices, int[][] edges, int[][] faces) {
            this.vertices = vertices.Select(v=>new Vertex(v)).ToArray();
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
            var tris = new int[faces.Sum(f => f.edges.Length - 2)];

            int fi = 0;
            int vi = 0;
            foreach(var face in faces) {
				var root = face.edges[0].a;
				verts[vi++] = root.position;
                verts[vi++] = face.edges[0].b.position;
                foreach(var edge in face.edges.Skip(1)) {
					verts[vi++] = edge.b.position	;
					tris[fi++] = vi-3;
					tris[fi++] = vi-2;
					tris[fi++] = vi-1;
                }
			}

            var mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;

            return mesh;
		}
    }
}
