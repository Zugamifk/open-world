using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Albedo.World;
using Albedo.Graphics;

namespace Albedo {
	public class PlayerControl : MonoBehaviour {

        [SerializeField]
        protected MovementControl m_movementControl;
        [SerializeField]
        protected float m_movementSpeed;
        [SerializeField]
        protected PlayerGraphicsController m_graphicsController;

        [Readonly][SerializeField]
		private Vector2 inputVelocity;

		[Readonly][SerializeField]
		private Vector2 velocity;

		[Readonly][SerializeField]
		protected Vector2 position;

	    private static PlayerControl instance;

		public static Vector2 Position {
			get {
                return PixelManager.SnapVector(instance.position);
            }
			protected set {
                // Debug.Log(instance.position + " -> " + value+" : "+Time.deltaTime+" : "+instance.velocity);
                instance.position = value;
            }
		}

        public static Rect BoundingBox = new Rect(-0.25f, -0.25f, .5f, .5f);
        public static Vector2[] Corners = new Vector2[] {
            new Vector2(-0.25f, -0.25f),
            new Vector2(-0.25f, 0.25f),
            new Vector2(0.25f, 0.25f),
            new Vector2(0.25f, -0.25f)
        };

        void Awake() {
            if (this.SetInstanceOrKill(ref instance))
            {
				m_movementControl.RegisterVelocityUpdate(UpdateMovementVelocity);
            }
        }

        void Start() {
            Position = Map.Middle;
			Albedo.Crunching.CrunchManager.UpdateStatus(Constants.PlayerControlInitialized, true);
        }

		void Update() {
	        UpdateVelocity();

            RaycastHit info;

            if (CheckCorners(out info))
            {
                var newvelocity = (Vector2)(info.point+info.normal*Constants.PixelSize*0.5f) - position;
                Debug.Log(info.point + " : " + info.normal+ " : "+(Vector2)(info.point+info.normal*Constants.PixelSize*2));

                position += newvelocity;
                velocity -= Vector2.Dot(-velocity, (Vector2)info.normal) * (Vector2)info.normal;
            } else {
            	position += velocity * Time.deltaTime;
			}
			Debugx.DrawRect(BoundingBox, Colorx.lightmaroon);
        }

		bool CheckCorners(out RaycastHit info) {
            RaycastHit hit;
            info = new RaycastHit();
            int collisionCorner = -1;
            for (int i = 0; i < 4; i++)
            {
                if(MapView.Raycast(position + Corners[i], position + Corners[i] + velocity * Time.deltaTime, an => an.Tile.Collides, out hit)) {
					if(collisionCorner<0 || hit.distance < info.distance) {
						info = hit;
                        Debug.Log(hit.distance);
                        collisionCorner = i;
                    }
				}
            }
			if(collisionCorner<0) {
				return false;
			} else
            {
                info.point -= (Vector3)Corners[collisionCorner];
				return true;
            }
        }

		void UpdateMovementVelocity(Vector2 velocity) {
			inputVelocity += velocity * m_movementSpeed * Time.deltaTime;
		}

		void UpdateVelocity() {
			velocity = inputVelocity;
			m_graphicsController.UpdateState(velocity);
            inputVelocity = Vector2.zero;
        }
	}
}
