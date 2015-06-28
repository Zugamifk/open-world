using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Graph<T> {
    private struct Edge {
        int from;
        int to;
    }
	public T[] Vertices;
    private Edge[] Edges;
	public bool IsDirected;

	public Graph(T[] vertices, int[][] edges) {

	}
}
