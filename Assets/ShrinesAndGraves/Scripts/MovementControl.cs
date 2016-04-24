using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
	public class MovementControl : MonoBehaviour {

        public class FootSensor : MonoBehaviour
        {
            public MovementControl.MoveEvent onCollision;
            void OnCollisionEnter2D(Collision2D collision)
            {
                if (onCollision != null)
                {
                    onCollision.Invoke();
                }
            }
        }

        public delegate void MoveUpdate(Vector2 move);
        MoveUpdate onMove;

        public delegate void MoveEvent();
        public MoveEvent onJump;
        public MoveEvent onHighJump;
        public MoveEvent onLand;
        public MoveEvent onFall;
        public delegate void ParamEvent(float t);
        public ParamEvent onJumpPowerUpdate;

        [Readonly][SerializeField]
        Vector2 move;

        public Rigidbody2D rigidbody;

        public float JumpPowerMax;
        public float InitialJumpPower;
        public float HighJumpTime;
        public float JumpTimeMax;
        public float FallThreshold;
        public float LandDelay;
        public float maxSpeed;
        public float acceleration;

        public Collider2D footCollider;
        [Layer]
        public int footColliderLayer;

        float jumpPower;
        float jumpTime;
        Vector2 jumpPosition;
        bool jumping, highJumping;

        Vector2 lastVelocity;

		void UpdateX(float val) {
			move.x+=val;
		}

        public Vector2 GetFootPosition()
        {
            return (Vector2)footCollider.transform.position + footCollider.offset + Vector2.down * .5f;
        }

        public void StartJump()
        {
            var grounded = footCollider.IsTouchingLayers(footColliderLayer);
            if (grounded)
            {
                jumpPower = 0;
                jumpTime = Time.fixedTime;
                rigidbody.AddForce(Vector2.up * InitialJumpPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
                jumpPosition = rigidbody.position;
                highJumping = false;
                jumping = true;
                if (onJump != null)
                {
                    onJump.Invoke();
                }
            }
        }

        public void Jump()
        {
            if (Time.fixedTime - jumpTime < JumpTimeMax)
            {
                var gain = 1 - (Time.fixedTime - jumpTime) / JumpTimeMax;
                rigidbody.AddForce(Vector2.up * JumpPowerMax * gain * gain * Time.fixedDeltaTime, ForceMode2D.Impulse);
                var newPower = (Time.fixedTime - jumpTime) / JumpTimeMax;
                if (newPower >= HighJumpTime * JumpTimeMax &&
                    jumpPower < HighJumpTime * JumpTimeMax)
                {
                    if(onHighJump != null)
                    { 
                        onHighJump.Invoke();
                    }
                    highJumping = true;
                }
                jumpPower = newPower;
                if(onJumpPowerUpdate!=null) {
                    onJumpPowerUpdate.Invoke(jumpPower);
                }
            }
        }

        void Land()
        {
            bool canLand = Time.fixedTime - jumpTime > LandDelay;
            if (canLand)
            {
                if (onLand != null)
                {
                    onLand.Invoke();
                }
                jumping = false;
            }
        }

        void Start() {
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, UpdateX);
            InputManager.RegisterButtonHeldCallback(InputKey.BUTTON0, Jump);
            InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, StartJump);

            if (footCollider != null)
            {
                var sensor = footCollider.gameObject.AddComponent<FootSensor>();
                sensor.onCollision += Land;
            }
		}

		void Update() {
            var vel = rigidbody.velocity;
            vel.x = Mathf.Clamp(vel.x + move.x * acceleration * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
            rigidbody.velocity = (Vector2f16)vel;

            if (lastVelocity.y >= -FallThreshold && rigidbody.velocity.y < -FallThreshold)
            {
                bool canFall = !jumping || highJumping || rigidbody.position.y < jumpPosition.y;
                if (!footCollider.IsTouchingLayers(footColliderLayer))
                {
                    if (onFall != null && canFall)
                    {
                        onFall.Invoke();
                    }
                }
            }

            lastVelocity = rigidbody.velocity;

            move = Vector2.zero;
		}
    }
}
