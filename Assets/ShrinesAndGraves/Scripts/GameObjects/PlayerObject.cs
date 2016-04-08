using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class PlayerObject : WorldObject
    {

        public Animator animator;
        public Transform graphicsRoot;

        MovementControl controller;
        public Rigidbody2D rigidbody;


        // Use this for initialization
        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            controller = gameObject.GetOrAddComponent<MovementControl>();

            collider = gameObject.GetOrAddComponent<CircleCollider2D>();
            collider.enabled = true;
        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2f16 pos)
        {
            position = pos;
            rigidbody.position = pos;
        }

        void Update()
        {
            position = rigidbody.position;

            var speed = rigidbody.velocity.x;
            if (Mathf.Abs(speed) > 0.05f)
            {
                if (speed < 0)
                {
                    graphicsRoot.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
                }
                else
                {
                    graphicsRoot.transform.localRotation = Quaternion.identity;
                }
            }
            animator.SetFloat("speed", Mathf.Abs(speed));
        }
    }
}