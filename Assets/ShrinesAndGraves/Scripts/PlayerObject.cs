using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class PlayerObject : WorldObject
    {

        MovementControl controller;

        PhysicsBody body;

        // Use this for initialization
        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            controller = FindObjectOfType<MovementControl>();
            controller.RegisterVelocityUpdate(Move);

            body = gameObject.GetOrAddComponent<PhysicsBody>();
            body.Initialize(new Rect(position, Vector2.one));
            Physics.registeredBodies.Add(body);
        }

        public delegate void PositionUpdateCallback(Vector2 position);
        public PositionUpdateCallback PositionUpdate;

        /** A normal move, limited by physics and gamer state */
        public void Move(Vector2 velocity)
        {
            position = body.position;
            if (PositionUpdate != null) PositionUpdate.Invoke(position);

            body.velocity += velocity * 10 * Time.deltaTime;
        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2 pos)
        {
            position = pos;
            body.position = pos;

            if (PositionUpdate != null) PositionUpdate.Invoke(position);
            //Debug.Log(pos + "->" + body.position + "->" + position);
        }
    }
}