using UnityEngine;
using System.Collections;
using System.Linq;
using Landscape;

public class FPSControl : MonoBehaviour
{

    public Transform controlRoot;
    public float acceleration;
    public float maxSpeed;

    private Vector3 velocity = Vector3.zero;

    private float speed = 0;
    private bool moving = false;
    private Vector3 moveVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    public static Vector3 Position { get; protected set; }

    private static FPSControl instance;
    void Awake() {
        this.SetInstanceOrKill(ref instance);
    }

    void Start() {
        controlRoot.position = Vector3.zero;
        Position = Vector3.zero;
    }

    void Update()
    {
        moveDirection = Vector3.zero;
        moving = false;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
            moving = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
            moving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
            moving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
            moving = true;
        }

        var sign = moving ? 1 : -1;
        speed = Mathf.Clamp(speed + acceleration * sign * Time.fixedDeltaTime, 0, maxSpeed);
        moveVelocity = moveDirection.normalized * speed;

        Position += (controlRoot.TransformVector(moveVelocity) + velocity) * Time.fixedDeltaTime;

        var pos = Position;
        pos.y = Ground.GetHeight(Position);
        Position = pos;
        gameObject.name = "player: "+Ground.TransformPoint(pos);;
    }

}
