using UnityEngine;
using System.Collections;
using System.Linq;

public class FPSControl : MonoBehaviour
{

    public Transform controlRoot;
    public MainPlayerCollider mainCollider;
    public float acceleration;
    public float maxSpeed;

    public AnimationCurve jumpImpulse;
    public float jumpMagnitude;
    public float jumpTime;

    private Vector3 velocity = Vector3.zero;

    private float speed = 0;
    private bool jumping = false;
    private bool moving = false;
    private Vector3 moveVelocity = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private FiniteStateMachine<State, StateVars> FSM;
    private enum State
    {
        GROUNDED, JUMPING
    }
    private struct StateVars : System.IComparable
    {
        public bool jumping;
        public int CompareTo(object o)
        {
            var v = (StateVars)o;
            return v.jumping == jumping ? 0 : 1;
        }
        public StateVars (bool jumping){
            this.jumping = jumping;
        }
    };

    private IEnumerator Jump()
    {
        jumping = false;
        for (float t = 0; t < 1; t += Time.deltaTime / jumpTime)
        {
            velocity += jumpMagnitude * jumpImpulse.Evaluate(t) * Vector3.up;
            yield return 1;
        }
    }

    void Start()
    {
        FSM = new FiniteStateMachine<State, StateVars>(
            State.JUMPING,
            state =>
            {
                switch (state)
                {
                    case State.GROUNDED:
                        return (System.Func<StateVars, State>)
                        (vars =>
                        {
                            if (vars.jumping)
                            {
                                StartCoroutine(Jump());
                                return State.JUMPING;
                            }
                            else
                            {
                                return State.GROUNDED;
                            }
                        }
                        );
                    case State.JUMPING:

                        velocity += PhysicsManager.Gravity;

                        return (System.Func<StateVars, State>)
                        (vars =>
                        {
                            return State.JUMPING;
                        }
                        );
                }
                return vars => state;
            }
        );
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

        if (Input.GetKey(KeyCode.Space))
        {
            jumping = true;
        }

        var sign = moving ? 1 : -1;
        speed = Mathf.Clamp(speed + acceleration * sign * Time.fixedDeltaTime, 0, maxSpeed);
        moveVelocity = moveDirection.normalized * speed;
        controlRoot.Translate((moveVelocity + velocity) * Time.fixedDeltaTime);
    }

}
