using UnityEngine;
using System.Collections;
using Extensions.Managers;

namespace Shrines
{
    public class PlayerObject : CharacterObject
    {

        MovementControl controller;

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
            m_rigidbody.isKinematic = false;
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
                jumpPower = m_rigidbody.velocity.y;
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

            InputManager.RegisterButtonDownCallback(InputKey.BUTTON1, Die);

        }

        /** Sets the position, ignoring game rules */
        public void SetPosition(Vector2f16 pos)
        {
            position = pos;
            m_rigidbody.position = pos;
        }

        public override void Die()
        {
            base.Die();

            WorldManager.DelayedRestart();
        }

        void Update()
        {
            position = m_rigidbody.position;

            var speed = m_rigidbody.velocity.x;
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

            if (animator.enabled)
            {
                if (jumping)
                {
                    jumpBehaviour.time = Mathf.InverseLerp(jumpPower, -jumpPower, m_rigidbody.velocity.y);
                }
                else
                {
                    animator.SetFloat("speed", Mathf.Abs(speed));
                }
            }
        }
    }
}