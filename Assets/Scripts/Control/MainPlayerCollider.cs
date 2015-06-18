using UnityEngine;
using System.Collections;

public class MainPlayerCollider : MonoBehaviour {

    private new Collider collider;
    public delegate void CollisionEvent(Collision c);
    public CollisionEvent OnCollisionEnterObserver;
    public CollisionEvent OnCollisionStayObserver;
    public Vector3 LastCollisionVector;

    private const float farRaycastPos = 100;

	void Start() {
        collider = GetComponent<Collider>();
    }

	private Vector3 GetCollisionPenetration(ContactPoint contact ) {
		var pos = contact.point;
		var nrm = contact.normal;
		RaycastHit info;
		Ray ray = new Ray(pos - farRaycastPos * nrm, nrm);
		if (collider.Raycast(ray, out info, farRaycastPos)) {
             return pos - info.point;
        }
        return Vector3.zero;
    }

    void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
			Debug.Log(contact.thisCollider.name + " at " + contact.normal);
            Debug.DrawRay(contact.point, contact.normal, Color.white, 5);
        }

        if(OnCollisionEnterObserver!=null)
			OnCollisionEnterObserver(collision);
    }

	void OnCollisionStay(Collision collision) {

        LastCollisionVector = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts) {
            Debug.Log(contact.thisCollider.name + " at " + contact.normal);
            Debug.DrawRay(contact.point, contact.normal, Color.white, 5);
            LastCollisionVector += GetCollisionPenetration(contact);
        }


		if(OnCollisionStayObserver!=null)
        	OnCollisionStayObserver(collision);
    }
}
