using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Geometry {
	public class Tile {
		protected class Vertex{
			public List<float[]> matchedRanges;
			public List<Edge> connected;
		}
		protected class Edge {
			public Vertex from;
			public Vertex to;
			public Tile left;
			public Tile right;
		}
		public Polygon shape;

		protected bool Match(Edge other) {
			return false; 
		}
	}
}
