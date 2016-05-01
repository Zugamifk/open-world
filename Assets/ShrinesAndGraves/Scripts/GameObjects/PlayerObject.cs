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

        public GameObject gibs;

        float jumpPower;
        bool jumping;
        ScriptedPlaybackBehaviour jumpBehaviour;

        // Use this for initialization
        public override void InitializeGameobject(Entity e)
        {
            graphicsRoot.gameObject.SetActive(true);

            collider = gameObject.GetComponent<BoxCollider2D>();

            base.InitializeGameobject(e);
            
            collider.enabled = true;
            rigidbody.isKinematic = false;
            animator.enabled = true;

            jumpBehaviour = animator.GetBehaviour<ScriptedPlaybackBehaviour>();

            controller = gameObject.GetOrAddComponent<MovementControl>();
            controller.onFall += () => animator.SetTrigger("fall");
            controller.onHighJump += () => animator.SetTrigger("high jump");
            controller.onJump += () =>
            {
                animator.SetTrigger("jump");
                animator.ResetTrigger("land");
                animator.ResetTrigger("fall");
                jumpPower = rigidbody.velocity.y;
                jumping = true;
            };
            controller.onLand += () =>
            {
                animator.SetTrigger("land");
                animator.ResetTrigger("jump");
                animator.ResetTrigger("high jump");
                animator.ResetTrigger("fall");
                jumping = false;
            };
            controller.onLedgeGrab += () =>
            {
                animator.SetTrigger("ledge grab");
                animator.ResetTrigger("ledge climb");
                animator.ResetTrigger("land");
            };
            controller.onLedgeClimb += () => {
                animator.SetTrigger("ledge climb");
            };
            
        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2f16 pos)
        {
            position = pos;
            rigidbody.position = pos;
        }

        public void Die()
        {
            animator.enabled = false; 
            graphicsRoot.gameObject.SetActive(false);
            collider.enabled = false;
            rigidbody.isKinematic = true;

            var go = (GameObject)Instantiate(gibs, graphicsRoot.transform.position, graphicsRoot.transform.rotation);
            var g = go.GetComponent<Gibs>();
            g.power = 1500;
            g.enabled = true;
            go.transform.SetParent(transform, true);

            WorldManager.DelayedRestart();
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

            if (jumping)
            {
                jumpBehaviour.time = Mathf.InverseLerp(jumpPower, -jumpPower, rigidbody.velocity.y);
            }
            else
            {
                animator.SetFloat("speed", Mathf.Abs(speed));
            }
        }
    }
}