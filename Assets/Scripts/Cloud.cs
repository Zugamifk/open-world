using UnityEngine;
using System.Collections;
using Geometry;

public class Cloud : MonoBehaviour {

	Polyhedron shape;
	Mesh ball;

	// Use this for initialization
	void Start () {
		shape = Shapes.Icosahedron();
		ball = shape.GenerateMesh();
		shape.ColorVertices(ball);
		MeshGenerator.Utils.PostGenerateMesh(ball);

		var mf = GetComponent<MeshFilter>();
		mf.mesh = ball;
	}
}
