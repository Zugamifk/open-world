using UnityEngine;
using System.Collections;

namespace Albedo {
	public class MovementControl : MonoBehaviour {
        public delegate void MovementControlVelocityUpdate(Vector2 move);
        private MovementControlVelocityUpdate velocityUpdate;

        [Readonly][SerializeField]
        private Vector2 velocity;

		public void RegisterVelocityUpdate(MovementControlVelocityUpdate update) {
            velocityUpdate += update;
        }

        public void UpdateY(float val) {
			velocity.y+=val;
		}

		public void UpdateX(float val) {
			velocity.x+=val;
		}

        void Start() {
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_HORIZONTAL, UpdateX);
			InputManager.RegisterAxisUpdateCallback(InputKey.MOVE_VERTICAL, UpdateY);
		}

		void Update() {
            if (velocityUpdate != null)
            {
                velocityUpdate(velocity);
            }

            velocity = Vector2.zero;
		}
    }
}
