using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class PlayerObject : WorldObject
    {

        public float JumpPower;

        MovementControl controller;

        PhysicsBody body;
        bool grounded;

        // Use this for initialization
        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            controller = gameObject.GetOrAddComponent<MovementControl>();
            controller.RegisterMoveUpdate(Move);
            controller.RegisterJumpHandler(Jump);

            body = gameObject.GetOrAddComponent<PhysicsBody>();
            body.Initialize(new Rect(0,0,1,1));
            body.OnCollide += OnCollide;
            Physics.registeredBodies.Add(body);
        }

        public delegate void PositionUpdateCallback(Vector2 position);
        public PositionUpdateCallback PositionUpdate;

        /** A normal move, limited by physics and gamer state */
        public void Move(Vector2 velocity)
        {
            if (PositionUpdate != null) PositionUpdate.Invoke(position);

            body.velocity += velocity * 10 * Time.fixedDeltaTime;
        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2f16 pos)
        {
            position = pos;
            body.position = pos;

            if (PositionUpdate != null) PositionUpdate.Invoke(position);
            //Debug.Log(pos + "->" + body.position + "->" + position);
        }

        public void OnCollide(Collision c)
        {
            //Debug.Log(c.data.normal);
            if (c.data.normal.y > 0)
            {
                grounded = true;
            }
        }

        public void Jump()
        {
            if (grounded)
            {
                body.velocity += Vector2.up * JumpPower * Time.fixedDeltaTime;
                grounded = false;
            }
        }

        void LateUpdate()
        {
            var newPos = body.position;
            if (newPos.y - position.y < -0.01f)
            {
                grounded = false;
            }
            position = newPos;
        }
    }
}