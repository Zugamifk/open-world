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

        new private SpriteRenderer renderer;

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
        new void Awake () {
            base.Awake();
            renderer = GetComponent<SpriteRenderer>();
            renderer.sortingOrder = Constants.PlayerSortingOrder;
            m_state = State.IDLE;
		}

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
