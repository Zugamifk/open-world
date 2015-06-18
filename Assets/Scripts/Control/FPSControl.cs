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

	private bool shouldResolveCollision = false;

	private void ResolveCollision(Collision c) {
		shouldResolveCollision = true;
    }

	private IEnumerator Jump() {
		for(float t=0;t<1;t+=Time.deltaTime/jumpTime) {
            velocity += jumpMagnitude * jumpImpulse.Evaluate(t)*Vector3.up;
			yield return 1;
        }
	}

	void Start() {
		mainCollider.OnCollisionEnterObserver += ResolveCollision;
		mainCollider.OnCollisionStayObserver += ResolveCollision;
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

		if (shouldResolveCollision) {
            controlRoot.Translate(
				mainCollider.LastCollisionVector
				* collisionCorrectionSpeed
				* Time.fixedDeltaTime
			);
			shouldResolveCollision = false;
            if (Vector3.Dot(mainCollider.LastCollisionVector, velocity) < 0)
            {
                velocity = Vector3.Reflect(velocity, mainCollider.LastCollisionVector.normalized) * (1 - collisionBounceDamp);
            }
        }
		controlRoot.Translate((moveVelocity+velocity) * Time.fixedDeltaTime);
	}

	void OnDrawGizmos() {

	}
}
