using UnityEngine;
using System.Collections;
using Geometry;

public class Cloud : MonoBehaviour {

	Mesh atom;

	// Use this for initialization
	void Start () {

		shape.ColorVertices(ball);
		MeshGenerator.Utils.PostGenerateMesh(ball);

		var mf = GetComponent<MeshFilter>();
		mf.mesh = ball;
	}
}
