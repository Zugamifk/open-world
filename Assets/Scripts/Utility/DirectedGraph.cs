using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DirectedGraph<T> : Graph<T> {
	public DirectedGraph(){}
	public DirectedGraph(T[] vertices, int[,] edges) {
		Vertices = vertices.Select(v=>new Vertex<T>(v)).ToList();
		Edges = new List<Edge<T>>();
		for(int e=0;e<edges.GetLength(0);e++) {
			AddEdge(
				Vertices[edges[e,0]],
				Vertices[edges[e,1]]
			 );
		}
	}
}
