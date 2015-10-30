using UnityEngine;
using System.Collections;
using Extensions;

namespace Albedo.Graphics {
	using World = Albedo.World;
	public class MapView : MonoBehaviour {
        [SerializeField]
        protected int tileWidth;
        [SerializeField]
        protected int tileHeight;

        private Vector2 centrePosition;

        private World.Tile[,] worldTiles;
        private Ground.Tile[,] groundTiles;

        private static MapView main;
        public static MapView Main {
            get
            {
                return main;
            }
        	protected set {
				main = value;
			}
        }

        public Rect ViewRect {
			get {
                return new Rect(
					centrePosition.x - tileWidth/2,
					centrePosition.y - tileWidth/2,
					tileWidth+1,
					tileHeight+1
                );
            }
		}

		public Vector2 CentrePosition {
			get {
                return centrePosition;
            }
		}

		void Awake() {
            worldTiles = new World.Tile[tileWidth + 1, tileHeight + 1];
            this.SetInstanceOrKill(ref main);
        }

		void Update() {
            centrePosition = PlayerControl.Position;
            Draw();
        }

		void Draw() {
			World.Map.GetTiles(ViewRect, ref worldTiles);
			for(int x=0;x<(int)ViewRect.width;x++) {
				for(int y=0;y<(int)ViewRect.height;y++) {
                    worldTiles[x, y].Draw(centrePosition);
                }
			}
		}

		void OnDrawGizmos(){
			if(!EditorProtection.IsPlaying) return;
			for(int x=0;x<(int)ViewRect.width;x++) {
				for(int y=0;y<(int)ViewRect.height;y++) {
                    var col = new ColorHSV(2*(centrePosition.x + (float)x) / ViewRect.width, Math.UNSin((float)y / ViewRect.height), 1);
                    Gizmos.color = (Color)col;
                    // Debug.Log(col);
                    Gizmos.DrawSphere(worldTiles[x,y].position-centrePosition, 0.25f);
                }
			}
		}
    }
}
