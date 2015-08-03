using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geometry {
	public class Tile {
		public class Vertex{
			public List<float[]> matchedRanges;
			public List<Edge> connected;
		}
		public class Edge {
			public Vertex from;
			public Vertex to;
			public Tile other;
		}
		public Polygon shape;
		public List<Vertex> verts;
		public List<Edge> edges;

		public Tile(Polygon s) {
			shape = s;
			verts = s.Vertices.Select(
				v=> {
					var vert = new Vertex();
					vert.matchedRanges = new List<float[]>();
					vert.connected = new List<Edge>();
					return vert;
				}
			).ToList();
			edges = s.Edges.Select(e=>{
				var edge = new Edge();
				edge.from = verts[s.Vertices.IndexOf(e.from)];
				edge.to = verts[s.Vertices.IndexOf(e.to)];
				return edge;
			}).ToList();
		}
	}
}
