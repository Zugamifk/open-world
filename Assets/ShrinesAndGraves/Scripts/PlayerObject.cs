using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class PlayerObject : WorldObject
    {

        public float JumpPower;

        MovementControl controller;

        new protected Rigidbody2D rigidbody;

        int groundPoints = 0;
        public bool grounded
        {
            get
            {
                return groundPoints > 0;
            }
        }

        // Use this for initialization
        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            controller = gameObject.GetOrAddComponent<MovementControl>();
            controller.RegisterMoveUpdate(Move);
            controller.RegisterJumpHandler(Jump);

            rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
            collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            collider.enabled = true;
            //body = gameObject.GetOrAddComponent<PhysicsBody>();
            //body.Initialize(new Rect(0,0,1,1));
            //body.OnCollide += OnCollide;
            //Physics.registeredBodies.Add(body);
        }

        public delegate void PositionUpdateCallback(Vector2 position);
        public PositionUpdateCallback PositionUpdate;

        /** A normal move, limited by physics and gamer state */
        public void Move(Vector2 velocity)
        {
            if (PositionUpdate != null) PositionUpdate.Invoke(position);

            rigidbody.velocity += velocity * 10 * Time.fixedDeltaTime;
        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2f16 pos)
        {
            position = pos;
            rigidbody.position = pos;

            if (PositionUpdate != null) PositionUpdate.Invoke(position);
            //Debug.Log(pos + "->" + body.position + "->" + position);
        }

        public void Jump()
        {
            if (grounded)
            {
                Debug.Log("blah");
                rigidbody.AddForce(Vector2.up * JumpPower * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }

        void OnCollisionEnter2D(Collision2D hit)
        {
            if (hit.contacts[0].normal.y > 0)
            {
                groundPoints++;
            }
        }

        void OnCollisionExit2D(Collision2D hit)
        {
            if (hit.contacts[0].normal.y > 0)
            {
                groundPoints--;
            }
        }

        void LateUpdate()
        {
            var newPos = rigidbody.position;

            position = newPos;
        }
    }
}