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
        Vector2 velocity;

		public void RegisterMoveUpdate(MoveUpdate update) {
            onMove += update;
        }

        public void RegisterJumpHandler(JumpEvent update)
        {
            onJump += update;
        }

		void UpdateX(float val) {
			velocity.x+=val;
		}

        void Jump()
        {
            if (onJump != null)
            {
                onJump();
            }
        }

        void Start() {
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, UpdateX);
            InputManager.RegisterButtonDownCallback(InputKey.BUTTON0, Jump);
		}

		void Update() {
            if (onMove != null)
            {
                onMove(velocity);
            }

            velocity = Vector2.zero;
		}
    }
}
