using UnityEngine;
using System.Collections;

public class LineBoxMeshMaker : MonoBehaviour {		
	void Awake(){
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		if(!mesh){
			mesh = new Mesh();
			mesh.name = "Generated Mesh";
		}
		float halfWidth = 0.5f;
		float halfHeight = 0.5f;
		Vector3[] verts = new Vector3[]{
			new Vector3(-halfWidth, -halfHeight, 0),
			new Vector3(halfWidth, -halfHeight, 0),
			new Vector3(halfWidth, halfHeight, 0),
			new Vector3(-halfWidth, halfHeight, 0)
		};
		Vector2[] uv = new Vector2[]{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		};
		mesh.vertices = verts;
		mesh.uv = uv;
		int[] indices = new int[8]{
			0, 1,
			1, 2,
			2, 3,
			3, 0
		};
		mesh.SetIndices(indices, MeshTopology.Lines, 0);
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}
}
