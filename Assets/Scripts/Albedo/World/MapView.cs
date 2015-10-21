using UnityEngine;
using System.Collections;

namespace Albedo.World {
	public class MapView : MonoBehaviour {
        [SerializeField]
        protected int tileWidth;
        [SerializeField]
        protected int tileHeight;

        private Vector2 centrePosition;

        private Tile[,] visibleTiles;

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

		void Awake() {
            visibleTiles = new Tile[tileWidth + 1, tileHeight + 1];
        }

		void Update() {
            centrePosition = PlayerControl.Position;
            Draw();
        }

		void Draw() {
			Map.GetTiles(ViewRect, ref visibleTiles);
			for(int x=0;x<(int)ViewRect.width;x++) {
				for(int y=0;y<(int)ViewRect.height;y++) {
                    visibleTiles[x, y].Draw(centrePosition);
                }
			}
		}
    }
}
