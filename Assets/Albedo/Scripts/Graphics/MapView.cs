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

        private TileAnimator[,] worldTiles;
		protected int visibleTilesWidth;
		protected int visibleTilesHeight;
		protected Vector3i bottomLeftTile;

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
					centrePosition.y - tileHeight/2,
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
			visibleTilesWidth = tileWidth + 1;
            visibleTilesHeight = tileHeight + 1;
            worldTiles = new TileAnimator[visibleTilesWidth, visibleTilesHeight];
			InitializeTiles();
            this.SetInstanceOrKill(ref main);
        }

		void Update() {
            centrePosition = PlayerControl.Position;
            UpdateTiles();
        }

		void OnDrawGizmos(){
			if(!EditorProtection.IsPlaying) return;
			Gizmos.color = Color.black;
            var rect = ViewRect;
			var ox = Math.Mod(centrePosition.x, 1);
            var oy = Math.Mod(centrePosition.y, 1);
            var offset = new Vector3(ox, oy, 0);
            for(int x=0;x<(int)ViewRect.width;x++) {
				Gizmos.DrawLine(new Vector3(-tileWidth/2+x, -tileHeight/2, 0)-offset, new Vector3(-tileWidth/2+x, tileHeight/2, 0)-offset);
			}
			for(int y=0;y<(int)ViewRect.height;y++) {
				Gizmos.DrawLine(new Vector3(-tileWidth/2, -tileHeight/2 + y, 0)-offset, new Vector3(tileWidth/2, -tileHeight/2 + y, 0)-offset);
			}
		}

		void InitializeTiles() {
			for(int x=0;x<visibleTilesWidth;x++) {
				for(int y=0;y<visibleTilesHeight;y++) {
                    var go = new GameObject();
					go.transform.OrientTo(TileDrawer.Root);
                    var tile = go.AddComponent<TileAnimator>();
                    worldTiles[x,y] = tile;
					tile.SetTile(World.Tile.EmptyTile());
				}
			}
		}

		void UpdateTiles() {
			Vector3i bl = GetTilePosition(ViewRect.position);
			if(bl!=bottomLeftTile) {
				bottomLeftTile = bl;
				RefreshTiles();
			}
			Vector3 offset = PlayerControl.Position;
			for(int x=0;x<visibleTilesWidth;x++) {
				for(int y=0;y<visibleTilesHeight;y++) {
					var tile = worldTiles[x, y];
					tile.transform.localPosition = tile.WorldPosition - offset;
				}
			}
		}

		Vector3i GetTilePosition(Vector2 position) {
			return new Vector3i(
                (int)position.x,
                (int)position.y,
				0
            );
		}

		void RefreshTiles() {
            var tiles = new World.Tile[visibleTilesWidth, visibleTilesHeight];
			World.Map.GetTiles(ViewRect, ref tiles);
            for (int x = 0; x < visibleTilesWidth; x++)
            {
                for (int y = 0; y < visibleTilesHeight; y++)
                {
					// Update current tile animator
                    var tile = worldTiles[x, y];
                    tile.SetTile(tiles[x,y]);
                }
            }
        }
    }
}
