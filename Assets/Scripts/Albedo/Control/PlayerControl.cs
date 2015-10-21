using UnityEngine;
using System.Collections;
using Albedo.World;

namespace Albedo {
	public class PlayerControl : MonoBehaviour {

        [SerializeField]
        protected MovementControl m_movementControl;
        [SerializeField]
        protected float m_movementSpeed;

		[Readonly][SerializeField]
		private Vector2 inputVelocity;

		[Readonly][SerializeField]
		private Vector2 velocity;

		[Readonly][SerializeField]
		protected Vector2 position;

	    private static PlayerControl instance;

		public static Vector2 Position {
			get {
                return instance.position;
            }
		}

	    void Awake() {
            if (this.SetInstanceOrKill(ref instance))
            {
				m_movementControl.RegisterVelocityUpdate(UpdateMovementVelocity);
            }
        }

		void Start() {
            position = Map.Middle;
        }

		void Update() {
	        UpdateVelocity();

			position += velocity * Time.deltaTime;
			Debugx.DrawCross(transform.position, 1, Colorx.lightmaroon);
        }

		void UpdateMovementVelocity(Vector2 velocity) {
			inputVelocity += velocity * m_movementSpeed * Time.deltaTime;
		}

		void UpdateVelocity() {
			velocity = inputVelocity;
            inputVelocity = Vector2.zero;
        }
	}
}
