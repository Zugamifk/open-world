using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {

	public Transform cameraRoot;
    public Transform playerRoot;
    public Vector2 sensitivity;

	private float rotationX;
    private float rotationY;
	private Quaternion originalCameraRotation;
    private Quaternion originalPlayerRotation;

    // Update is called once per frame
    void Update () {
		// Read the mouse input axis
		rotationX += Input.GetAxis("Mouse X") * sensitivity.x;
		rotationY += Input.GetAxis("Mouse Y") * sensitivity.y;

		rotationX = ClampAngle (rotationX, -360, 360);
		rotationY = ClampAngle (rotationY, -90, 90);

		Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);

		cameraRoot.localRotation = originalCameraRotation * yQuaternion;
		playerRoot.localRotation = originalPlayerRotation * xQuaternion;
    }

	void Start() {
		originalCameraRotation = cameraRoot.localRotation;
		originalPlayerRotation = playerRoot.localRotation;
    }

	public float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}
