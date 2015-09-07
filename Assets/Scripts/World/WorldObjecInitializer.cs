using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldObjecInitializer : MonoBehaviour {

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	public void Clear() {
		meshFilter.mesh = null;
	}

	public void Initialize(WorldObject obj) {
		meshFilter.mesh = obj.mesh;
		meshRenderer.material = obj.material;
		transform.localPosition = obj.position;
		gameObject.name = obj.name;

		obj.root = transform;
	}

	void Awake() {
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
	}
}
