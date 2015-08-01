using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DirectedGraph<T> : Graph<T> {
	public DirectedGraph(){}
	public DirectedGraph(T[] vertices, int[,] edges) {
		Vertices = vertices.Select(v=>new Vertex<T>(v)).ToList();
		Edges = new HashSet<Edge<T>>();
		for(int e=0;e<edges.GetLength(0);e++) {
			AddEdge(
				Vertices[edges[e,0]],
				Vertices[edges[e,1]]
			 );
		}
	}

	public void AddEdge(Vertex<T> v0, Vertex<T> v1) {
		var edge = new Edge<T> (v0,v1);
		Edges.Add(edge);
		v0.edges.Add(edge);
		v1.edges.Add(edge);
	}
}
