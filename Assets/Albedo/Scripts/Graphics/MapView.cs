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

        [Header("Debug")]
        [SerializeField]
        bool drawGrid;

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
            get;
         	protected set;
	 	}

		public Vector2 CentrePosition {
			get {
                return centrePosition;
            }
		}

		public static World.Tile Selected {
			get; protected set;
		}

		public static TileAnimator GetTileAnimator(Vector3i position) {
            position -= main.bottomLeftTile;
			if(	position.x<0 || position.x>=main.worldTiles.GetLength(0) ||
				position.y<0 || position.y>=main.worldTiles.GetLength(1)) {
				return null;
			}
            return main.worldTiles[position.x, position.y];
        }

		public static bool Raycast(Vector2 start, Vector2 end, System.Func<TileAnimator, bool> test, out RaycastHit info) {
			info = new RaycastHit();
            if (Math.Approximately(start, end))
            {
				info.point = end;
                return false;
            }

            var line = end - start;
            var normal = Vector2.zero;
            float xm = -1, ym = -1;
            bool hit = false;
            foreach(var position in Math2D.SuperCover(start, end)) {
				var an = GetTileAnimator((Vector3i)position);
                if (an != null)
                {
                    if (test(an))
                    {
                        var to = position - start;
                        var bl = -to;
                        if (to.x < 0)
                        {
                            to.x += 1;
                            xm = 1;
                        }
                        if (to.y < 0)
                        {
                            to.y += 1;
                            ym = 1;
                        }
                        var dx = Mathf.Abs(to.x / line.x);
                        var dy = Mathf.Abs(to.y / line.y);
                        if (Mathf.Approximately(line.y,0) || (bl.y > 0 && bl.y < 1))
                        {
                            line *= dx;
							normal = Vector3.right*xm;
                        }
                        else if (Mathf.Approximately(line.x,0) || (bl.x > 0 && bl.x < 1))
                        {
                            line *= dy;
							normal = Vector3.up*ym;
                        }
                        else
                        {
							if(dx>dy) {
								line*=dx;
								normal = Vector3.right*xm;
							} else {
								line*=dy;
								normal = Vector3.up*ym;
							}
                        }
						hit = true;
                        break;
                    }
                }
            }
            info.point = start + line;
            info.distance = line.magnitude;
            info.normal = normal;
            return hit;
		}

		void Awake() {
			visibleTilesWidth = tileWidth + 1;
            visibleTilesHeight = tileHeight + 1;

			ViewRect = new Rect(
				centrePosition.x - tileWidth/2,
				centrePosition.y - tileHeight/2,
				visibleTilesWidth,
				visibleTilesHeight
			);

            worldTiles = new TileAnimator[visibleTilesWidth, visibleTilesHeight];

            this.SetInstanceOrKill(ref main);
        }

		void Start() {
			InitializeTiles();
		}

		void Update() {
            centrePosition = PlayerControl.Position;

            var mouseHoverPos = Camera.main.ScreenToWorldPoint(InputManager.MouseScreenPosition);
			Debugx.DrawCross(mouseHoverPos, 0.3f, Color.red);
            Vector3i selectedPos = Vector3i.RoundDown(mouseHoverPos + (Vector3)centrePosition);
            // Selected = World.Map.GetTile(selectedPos);

            RaycastHit info;
            Raycast(centrePosition, (Vector2)mouseHoverPos+centrePosition, an => an.Tile.Collides, out info);
			Debug.DrawLine(Vector3.zero, (Vector2)info.point-centrePosition, Color.yellow);

            UpdateTiles();
        }

		void LateUpdate() {
			ViewRect = new Rect(
				centrePosition.x - tileWidth/2,
				centrePosition.y - tileHeight/2,
				tileWidth+1,
				tileHeight+1
			);
		}

		void OnDrawGizmos(){
			if(!EditorProtection.IsPlaying) return;
            if (drawGrid)
            {
                Gizmos.color = Color.black;
                var ox = Math.Mod(centrePosition.x, 1);
                var oy = Math.Mod(centrePosition.y, 1);
                var offset = new Vector3(ox, oy, 0);
                for (int x = 0; x < (int)ViewRect.width; x++)
                {
                    Gizmos.DrawLine(new Vector3(-tileWidth / 2 + x, -tileHeight / 2, 0) - offset, new Vector3(-tileWidth / 2 + x, tileHeight / 2, 0) - offset);
                }
                for (int y = 0; y < (int)ViewRect.height; y++)
                {
                    Gizmos.DrawLine(new Vector3(-tileWidth / 2, -tileHeight / 2 + y, 0) - offset, new Vector3(tileWidth / 2, -tileHeight / 2 + y, 0) - offset);
                }
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
