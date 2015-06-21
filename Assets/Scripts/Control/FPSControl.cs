using UnityEngine;
using System.Collections;
using System.Linq;

public class FPSControl : MonoBehaviour {

    public Transform controlRoot;
    public MainPlayerCollider mainCollider;
    public float acceleration;
    public float maxSpeed;
    public float collisionCorrectionSpeed;

    public AnimationCurve jumpImpulse;
    public float jumpMagnitude;
    public float jumpTime;

    public float collisionBounceDamp=0;

    private Vector3 velocity = Vector3.zero;

    private float speed = 0;
    private bool moving = false;
	private Vector3 moveVelocity = Vector3.zero;
	private Vector3 moveDirection = Vector3.zero;
    private const float farRaycastPos = 100;

    private Vector3 correction;
    private bool correcting = false;

    private void ResolveCollision(Collision c) {
        foreach (ContactPoint contact in c.contacts)
        {
            // Debug.Log(contact.thisCollider.name + " at " + contact.normal);
            Debug.DrawRay(contact.point, contact.normal, Color.white, 5);
            correction += GetContactPenetration(contact);
        }
        Debug.Log("Contacts: " + c.contacts.Length);
        correcting = true;
    }

    private Vector3 GetContactPenetration(ContactPoint contact ) {
		var pos = contact.point;
		var nrm = contact.normal;
		RaycastHit info;
		Ray ray = new Ray(pos - farRaycastPos * nrm, nrm);
		if (contact.thisCollider.Raycast(ray, out info, farRaycastPos)) {
             return pos - info.point;
        }
        return Vector3.zero;
    }

    private IEnumerator Jump() {
		for(float t=0;t<1;t+=Time.deltaTime/jumpTime) {
            velocity += jumpMagnitude * jumpImpulse.Evaluate(t)*Vector3.up;
			yield return 1;
        }
	}

	void Start() {

	}

    void Update() {
        moveDirection = Vector3.zero;
		moving = false;
        if (Input.GetKey(KeyCode.W)) {
			moveDirection += Vector3.forward;
			moving = true;
        }

		if (Input.GetKey(KeyCode.A)) {
			moveDirection += Vector3.left;
			moving = true;
        }

		if (Input.GetKey(KeyCode.S)) {
			moveDirection += Vector3.back;
			moving = true;
        }

		if (Input.GetKey(KeyCode.D)) {
			moveDirection += Vector3.right;
			moving = true;
        }

		if (Input.GetKey(KeyCode.Space)) {
            StartCoroutine(Jump());
        }

	}

	void FixedUpdate() {
		var sign = moving ? 1 : -1;
        speed = Mathf.Clamp(speed + acceleration * sign * Time.fixedDeltaTime, 0, maxSpeed);
		moveVelocity = moveDirection.normalized * speed;

		velocity += PhysicsManager.Gravity;

		if (correcting) {
            Debug.Log(correction);
            Debug.DrawLine(controlRoot.position, controlRoot.position+correction, Colorx.darkblue, 5);
            controlRoot.position += correction;
            if (Vector3.Dot(correction, velocity) < 0)
            {
                velocity = Vector3.Reflect(velocity, correction.normalized) * (1 - collisionBounceDamp);
            }
        }
		controlRoot.Translate((moveVelocity+velocity) * Time.fixedDeltaTime);

        correcting = false;
        correction = Vector3.zero;
    }

}
