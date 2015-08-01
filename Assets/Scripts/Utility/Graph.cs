using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Graph<T> : IUnitTestable {
    public class Vertex<Tvalue> {
        public Tvalue value;
        public List<Edge<Tvalue>> edges = new List<Edge<Tvalue>>();
        public Vertex(Tvalue val) {
            value = val;
        }
        public IEnumerable<Vertex<Tvalue>> connected {
            get {
                return edges.Select(e=>e.to==this?e.from:e.to);
            }
        }
        public bool ConnectedTo(Vertex<Tvalue> other) {
            return connected.FirstOrDefault(c=>c==other) != null;
        }

        public override string ToString() {
            return "Vertex "+value;
        }
    }
    public class Edge<Tvertex> {
        public Vertex<Tvertex> from;
        public Vertex<Tvertex> to;
        public Edge(Vertex<Tvertex> a, Vertex<Tvertex> b) {
            from = a;
            to = b;
        }
        public bool ConnectedTo(Edge<Tvertex> other) {
            return from==other.from || from == other.to
                || to == other.from || to == other.to;
        }
        public override string ToString() {
            return "Edge:\n\t"+from+"\n\t"+to;
        }
    }
	public Vertex<T>[] Vertices;
    public HashSet<Edge<T>> Edges;

    public Graph(){}
	public Graph(T[] vertices, int[,] edges) {
        Vertices = vertices.Select(v=>new Vertex<T>(v)).ToArray();
        Edges = new HashSet<Edge<T>>();
        for(int e=0;e<edges.GetLength(0);e++) {
            var newEdge = new Edge<T>(
                Vertices[Mathf.Min(edges[e,0],edges[e,1])],
                Vertices[Mathf.Max(edges[e,0],edges[e,1])]
             );
            Edges.Add(newEdge);
            Vertices[edges[e,0]].edges.Add(newEdge);
            Vertices[edges[e,1]].edges.Add(newEdge);
        }
        visited = new HashSet<Vertex<T>>();
	}

    private HashSet<Vertex<T>> visited;
    public IEnumerable<T> DFS {
        get {
            visited = new HashSet<Vertex<T>>();
            return DFSIterator(Vertices[0]);
        }
    }

    private IEnumerable<T> DFSIterator(Vertex<T> current) {
        visited.Add(current);
        yield return current.value;
        var connected = current.connected
            .Where(v=>!visited.Contains(v))
            .SelectMany(v=>DFSIterator(v));
        foreach(var next in connected) {
            yield return next;
        }
    }

    public virtual void Test() {
        var testgraph = new Graph<int>(
            new int[]{0,1,2,3,4},
            new int[,]{{0,1},{1,2},{2,3},{3,4},{4,0}}
        );
        foreach(var v in testgraph.DFS) {
            Debug.Log("Visited "+v);
        }
    }
}
