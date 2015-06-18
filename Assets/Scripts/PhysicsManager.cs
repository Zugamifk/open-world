using UnityEngine;
using System.Collections;

public class PhysicsManager : MonoBehaviour {

	public static Vector3 Gravity {
		get {
            return Physics.gravity * Time.fixedDeltaTime;
        }
	}

    private static PhysicsManager s_instance;

	void Awake() {
		if (s_instance!=null) {
            Debug.Log("Multiple PhysicsMangers! Self-destructing", s_instance);
            Destroy(this);
            return;
        }
		s_instance = this;
	}

}
