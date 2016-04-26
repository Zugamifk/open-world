using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
	public class MovementControl : MonoBehaviour {

        enum MoveMode
        {
            Free,
            Climbing
        }

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

        public class HandSensor : TriggerReceiver
        {
            public MovementControl.MoveEvent onLedgeGrab;
            public EntityTrigger lastTrigger;
            public override void OnEnterTrigger(EntityTrigger trigger)
            {
                var lt = trigger as TileObject.LedgeTrigger;
                lastTrigger = trigger;
                if (lt != null && onLedgeGrab != null)
                {
                    onLedgeGrab.Invoke();
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
        public MoveEvent onLedgeGrab;
        public MoveEvent onLedgeClimb;
        public MoveEvent onLedgeDrop;
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

        public Collider2D handsCollider;
        public Collider2D footCollider;
        [Layer]
        public int footColliderLayer;

        float jumpPower;
        float jumpTime;
        Vector2 jumpPosition;
        bool jumping, highJumping;
        
        HandSensor handSensor;

        MoveMode currentMovementMode = MoveMode.Free;

        Vector2 lastVelocity;

        bool blockInput;

		void UpdateX(float val) {
            if (blockInput) return;
            move.x+=val;
		}

        public Vector2 GetFootPosition()
        {
            return (Vector2)footCollider.transform.position + footCollider.offset + Vector2.down * .5f;
        }

        public void StartJump()
        {
            if (blockInput) return;
            switch (currentMovementMode)
            {
                case MoveMode.Free:
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
                    } break;
                case MoveMode.Climbing:
                    {
                        ClimbLedge();
                    }
                    break;
                default:
                    break;
            }
        }

        IEnumerator TeleportToLedge()
        {
            SetMoveMode(MoveMode.Free);
            blockInput = true;
            yield return new WaitForSeconds(.3f);
            transform.position = handSensor.lastTrigger.transform.position;
            yield return new WaitForSeconds(.5f);
            blockInput = false;
            SetColliders(true);            
        }

        public void Jump()
        {
            if (blockInput) return;
            switch (currentMovementMode)
            {
                case MoveMode.Free:
                    {
                        if (Time.fixedTime - jumpTime < JumpTimeMax)
                        {
                            var gain = 1 - (Time.fixedTime - jumpTime) / JumpTimeMax;
                            rigidbody.AddForce(Vector2.up * JumpPowerMax * gain * gain * Time.fixedDeltaTime, ForceMode2D.Impulse);
                            var newPower = (Time.fixedTime - jumpTime) / JumpTimeMax;
                            if (newPower >= HighJumpTime * JumpTimeMax &&
                                jumpPower < HighJumpTime * JumpTimeMax)
                            {
                                if (onHighJump != null)
                                {
                                    onHighJump.Invoke();
                                }
                                highJumping = true;
                            }
                            jumpPower = newPower;
                            if (onJumpPowerUpdate != null)
                            {
                                onJumpPowerUpdate.Invoke(jumpPower);
                            }
                        }
                    }
                    break;
                case MoveMode.Climbing:
                    break;
                default:
                    break;
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

        void GrabLedge()
        {
            if (onLedgeGrab != null)
            {
                onLedgeGrab.Invoke();
            }
            StartCoroutine(DelayedBlockInput(.2f));
            transform.position = handSensor.lastTrigger.transform.position - (Vector3)handsCollider.transform.localPosition;
            SetMoveMode(MoveMode.Climbing);
            SetColliders(false);
        }

        void ClimbLedge()
        {
            if (onLedgeClimb != null)
            {
                onLedgeClimb.Invoke();
            }
            StartCoroutine(TeleportToLedge());
        }

        void SetColliders(bool to)
        {
            handsCollider.enabled = to;
            footCollider.enabled = to;
        }

        void SetMoveMode(MoveMode mode)
        {
            if (mode == currentMovementMode) return;
            switch (mode)
            {
                case MoveMode.Free:
                    {
                        rigidbody.isKinematic = false;
                    }
                    break;
                case MoveMode.Climbing:
                    {
                        rigidbody.isKinematic = true;
                        rigidbody.velocity = Vector3.zero;
                    }
                    break;
                default:
                    break;
            }
            currentMovementMode = mode;
        }

        IEnumerator DelayedBlockInput(float time)
        {
            blockInput = true;
            yield return new WaitForSeconds(time);
            blockInput = false;
        }

        void Start() {
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, UpdateX);
            InputManager.RegisterButtonHeldCallback(InputKey.BUTTON0, Jump);
            InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, StartJump);

            if (handsCollider != null)
            {
                var sensor = handsCollider.gameObject.AddComponent<HandSensor>();
                sensor.onLedgeGrab += GrabLedge;
                handSensor = sensor;
            }

            if (footCollider != null)
            {
                var sensor = footCollider.gameObject.AddComponent<FootSensor>();
                sensor.onCollision += Land;
            }
		}

		void FixedUpdate() {
            if (blockInput) move = Vector2.zero;
            switch (currentMovementMode)
            {
                case MoveMode.Free:
                    {
                        var vel = rigidbody.velocity;
                        vel.x = Mathf.Clamp(vel.x + move.x * acceleration * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
                        rigidbody.velocity = (Vector2f16)vel;
                        if (lastVelocity.y >= -FallThreshold && rigidbody.velocity.y < -FallThreshold)
                        {
                            if (!footCollider.IsTouchingLayers(footColliderLayer))
                            {
                                if (onFall != null)
                                {
                                    onFall.Invoke();
                                }
                            }
                        }
                    } break;
                case MoveMode.Climbing:
                    {
                        var edgeSide = handSensor.lastTrigger.transform.position - transform.position;
                        if (edgeSide.x * move.x < -0.5f) // moving opposite direction , fall
                        {
                            SetMoveMode(MoveMode.Free);
                            if (onFall != null)
                            {
                                onFall.Invoke();
                            }
                        }
                        else if (edgeSide.x * move.x > 0.5f)
                        {
                            ClimbLedge();
                        }
                    }
                    break;
                default:
                    break;
            }

            lastVelocity = rigidbody.velocity;
            move = Vector2.zero;

		}
    }
}
