using UnityEngine;
using System.Collections;
using Extensions;

namespace Albedo.Graphics {
	public class PlayerGraphicsController : GraphicManager {

		public enum State {
			IDLE,
			WALK
		}

		[SerializeField][Readonly]
        private State m_state;

        protected State state {
			get {
				return m_state;
        	}
			set {
                if (m_state != value)
                {
                    m_animator.SetInteger("Action State", (int)value);
                    m_state = value;
                }
            }
    	}

        // Use this for initialization
        void Awake () {
            base.Awake();
            m_state = State.IDLE;
		}

        private Vector2 lastVelocity = Vector2.zero;
        public void UpdateState(Vector2 velocity) {
			if(!Math.Approximately(velocity, Vector2.zero)) {
				SetDirection(velocity);
				state = State.WALK;
			} else {
				state = State.IDLE;
			}
		}

	}
}
