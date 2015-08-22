using UnityEngine;
using System.Collections;

public class RotationTest : MonoBehaviour {

	public Transform before, after;
	public float angle = 5;

	void Update() {
		before.rotation = Quaternion.AngleAxis(angle*Time.deltaTime, Vector3.forward) * before.rotation;
		after.rotation = after.rotation * Quaternion.AngleAxis(angle*Time.deltaTime, Vector3.forward);
	}

}
