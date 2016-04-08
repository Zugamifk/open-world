using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
	public class MovementControl : MonoBehaviour {
        public delegate void MoveUpdate(Vector2 move);
        MoveUpdate onMove;

        public delegate void JumpEvent();
        JumpEvent onJump;

        [Readonly][SerializeField]
        Vector2 move;

        public Rigidbody2D rigidbody;

        public float JumpPower;
        public float maxSpeed;
        public float acceleration;

        protected Collider2D collider;


        int groundPoints = 0;
        public bool grounded
        {
            get
            {
                return groundPoints > 0;
            }
        }

		void UpdateX(float val) {
			move.x+=val;
		}

        public Vector2 GetFootPosition()
        {
            return (Vector2)collider.transform.position + collider.offset + Vector2.down * .5f;
        }

        public void Jump()
        {
            var grounded = Physics2D.Raycast(GetFootPosition(), Vector2.down, 0.1f);
            Debug.DrawLine(GetFootPosition(), GetFootPosition()+Vector2.down * 0.1f, Color.red);
            if (grounded.collider!=null)
            {
                rigidbody.AddForce(Vector2.up * JumpPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
            }
        }

        void Start() {
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, UpdateX);
            InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, Jump);

            collider = gameObject.GetOrAddComponent<CircleCollider2D>();
		}

		void Update() {
            var vel = rigidbody.velocity;
            vel.x = Mathf.Clamp(vel.x + move.x * acceleration * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
            rigidbody.velocity = (Vector2f16)vel;

            move = Vector2.zero;
		}
    }
}
