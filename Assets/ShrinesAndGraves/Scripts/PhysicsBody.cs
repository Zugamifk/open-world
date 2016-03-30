using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class PhysicsBody : MonoBehaviour, IDebuggable
    {
        public Vector2 acceleration;
        public Vector2 velocity;
        public Vector2 position;

        public Vector2[] corners;

        public delegate void CollisionEvent(Collision c);
        public CollisionEvent OnCollide;
        
        public Rect rect
        {
            get
            {
                return new Rect(position, corners[2]);
            }
        }

        public void PositionUpdate(Vector2 position)
        {
            var diff = position - this.position;
            for (int i = 0; i < 4; i++)
            {
                corners[i] += diff;
            }
            this.position = position;
        }

        public void Initialize(Rect rect)
        {
            corners = new Vector2[] {
                rect.position,
                new Vector2(rect.x, rect.yMax),
                rect.position + rect.size,
                new Vector2(rect.xMax, rect.y)
            };
        }

        public void OnCollision(Collision collision)
        {
            if (OnCollide != null)
            {
                OnCollide(collision);
            }
        }

        public void GetDebugMessageArgs(out string format, out System.Func<object>[] args)
        {
            format = "Position: {0}\nVelocity: {1}\nAcceleration: {2}";
            args = new System.Func<object>[] {
                () => position,
                () => velocity,
                () => acceleration
            };
        }

        public void DebugDraw(Vector2 offset = default(Vector2))
        {
            Debugx.DrawRect(new Rect(position - offset, corners[2]), Color.green);
        }
    }
}