using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {

	public enum Direction {
		UP,
		DOWN,
		RIGHT,
		LEFT,
		NUM
	}
	public class GraphicManager : MonoBehaviour {

		[SerializeField][Readonly]
        protected Direction m_direction;

        protected Animator m_animator;
        public Direction Direction {
			get {
				return m_direction;
			}
			set{
                if (value != m_direction)
                {
                    m_animator.SetInteger("Direction", (int)value);
                    m_direction = value;
                }
            }
		}

		public void SetDirection(Vector2 facing) {
			m_animator.SetFloat("X", facing.x);
			m_animator.SetFloat("Y", facing.y);
        }

		protected void Awake() {
            m_animator = GetComponent<Animator>();
        }
	}
}
