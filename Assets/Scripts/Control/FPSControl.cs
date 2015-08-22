using UnityEngine;
using System.Collections;
using System.Linq;
using Landscape;

public class FPSControl : MonoBehaviour
{

    public Transform controlRoot;
    public float acceleration;
    public float maxSpeed;
    public float jumpPower;

    private Vector3 velocity = Vector3.zero;

    private float speed = 0;
    private bool moving = false;
    private Vector3 moveVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    private bool grounded; // can you jump?

    public static Vector3 Position { get; protected set; }

    public void Jump() {
        if(!grounded) return;
        velocity += jumpPower*Vector3.up;
        grounded = false;
    }

    public void GetInput() {
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

        if (Input.GetKey(KeyCode.Space)) {
            Jump();
        }
    }

    void UpdateMovement() {
        var sign = moving ? 1 : -1;
        speed = Mathf.Clamp(speed + acceleration * sign * Time.fixedDeltaTime, 0, maxSpeed);
        moveVelocity = moveDirection.normalized * speed;
    }

    void UpdateVelocity() {
        if(!grounded){
            velocity+=PhysicsManager.Gravity;
        }
    }

    void UpdatePosition() {
        Position += (controlRoot.TransformVector(moveVelocity) + velocity) * Time.fixedDeltaTime;
        var pos = Position;
        var groundy = Ground.GetHeight(Position);
        if(pos.y < groundy) {
            pos.y = groundy;
            velocity = Vector3.zero;
            grounded = true;
        }
        Position = pos;
    }

    private static FPSControl instance;
    void Awake() {
        this.SetInstanceOrKill(ref instance);
    }

    void Start() {
        controlRoot.position = Vector3.zero;
        Position = new Vector3(0.1f,0f,0.1f);
        Ground.GetHeight(Position);
    }

    void FixedUpdate()
    {
        // reset some per-frame values
        moveDirection = Vector3.zero;
        moving = false;

        GetInput();

        UpdateMovement();
        UpdateVelocity();
        UpdatePosition();

        gameObject.name = "player: "+Ground.TransformPoint(Position);
    }

}
