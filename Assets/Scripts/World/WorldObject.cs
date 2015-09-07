using UnityEngine;
using System.Collections;

public class WorldObject {
	public Vector3 position;
	public Mesh mesh;
	public Material material;
	IWorldObject data;

	public string name;

	public void Initialize(IWorldObject obj) {
		data = obj;
		obj.InitializeWithWorldObject(this);
		mesh = obj.mesh;
		name = obj.mesh.name;
	}

	public Transform root;
}
