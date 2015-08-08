using UnityEngine;
using System.Collections;

public class PhysicsManager : MonoBehaviour {

	public float gravityScalar = 1;

	public static Vector3 Gravity {
		get {
            return Physics.gravity * s_instance.gravityScalar * Time.fixedDeltaTime;
        }
	}

    private static PhysicsManager s_instance;
	void Awake() {
		this.SetInstanceOrKill(ref s_instance);
	}

}
