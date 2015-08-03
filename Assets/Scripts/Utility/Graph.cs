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
	public List<Vertex<T>> Vertices;
    public List<Edge<T>> Edges;

    public Graph(){}
	public Graph(T[] vertices, int[,] edges) {
        Vertices = vertices.Select(v=>new Vertex<T>(v)).ToList();
        Edges = new List<Edge<T>>();
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

    public void AddEdge(Vertex<T> v0, Vertex<T> v1) {
        var edge = new Edge<T> (v0,v1);
        Edges.Add(edge);
        v0.edges.Add(edge);
        v1.edges.Add(edge);
    }

    public void RemoveEdge(Vertex<T> v0, Vertex<T> v1) {
        for(int i=0;i<Edges.Count;i++) {
            if((Edges[i].from == v0 && Edges[i].to == v1) ||
                (Edges[i].to == v0 && Edges[i].from == v1)) {
                Edges.RemoveAt(i);
            }
        }
        v0.edges = v0.edges.Where(e=>e.to!=v1 && e.from!=v1).ToList();
        v1.edges = v1.edges.Where(e=>e.to!=v0 && e.from!=v0).ToList();
    }

    public void AddVertex(T vert) {
        Vertices.Add(new Vertex<T>(vert));
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
