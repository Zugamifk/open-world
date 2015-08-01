using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DirectedGraph<T> : Graph<T> {
	public DirectedGraph(){}
	public DirectedGraph(T[] vertices, int[,] edges) {
		Vertices = vertices.Select(v=>new Vertex<T>(v)).ToArray();
		this.edges = new HashSet<Edge<T>>();
		for(int e=0;e<edges.GetLength(0);e++) {
			var newEdge = new Edge<T>(
				Vertices[edges[e,0]],
				Vertices[edges[e,1]]
			 );
			this.edges.Add(newEdge);
			Vertices[edges[e,0]].edges.Add(newEdge);
			Vertices[edges[e,1]].edges.Add(newEdge);
		}
	}
}
