using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Textures;
using Albedo;
using Albedo.Crunching;

namespace Albedo.Graphics {

	using World = Albedo.World;
	public class Ground : MonoBehaviour {

        [SerializeField]
        protected int tileWidth;
        [SerializeField]
        protected int tileHeight;
        [SerializeField]
        protected Sprite testSprite;
        [SerializeField]
        protected Gradient gradient;
        [SerializeField][Tooltip("Radius around start rect to generate tiles")]
        protected int precrunchRadius = 0;
        [SerializeField]
        [Readonly]
        protected int generatedTileCount;

        protected GroundTexture groundTextureGenerator;
        protected Dictionary<Vector3i, Sprite> spriteLookup;
		protected int visibleTilesWidth;
        protected int visibleTilesHeight;
        protected Tile[,] currentTiles;
		protected World.Tile[,] worldTilesforGroundTile;
        protected Vector3i bottomLeftTile;

        public class Tile : MonoBehaviour {
            protected World.Tile[,] contained;
            new protected SpriteRenderer renderer;
            protected Vector3i worldPosition;

			public Vector3i WorldPosition {
				get {
                    return worldPosition;
				}
				set{
					name = string.Format("Ground Tile [{0}, {1}]", value.x, value.y);
					worldPosition = value;
				}
			}

            void Awake() {
                renderer = GetComponent<SpriteRenderer>();
            }

			public void SetTiles(World.Tile[,] tiles) {
				contained = tiles;
			}

			public void SetSprite(Sprite sprite) {
				renderer.sprite = sprite;
			}

        }

        private Rect? _tileRect;
        public Rect TilePixelRect {
			get {
				if(!_tileRect.HasValue) {
                    _tileRect = new Rect(
                        0, 0,
                        Constants.TileWidthPixels * tileWidth,
                        Constants.TileHeightPixels * tileHeight
                    );
                }
                return _tileRect.Value;
            }
		}

		void Awake() {
            groundTextureGenerator = new GroundTexture(tileWidth, tileHeight);
            groundTextureGenerator.Colors = gradient;
            spriteLookup = new Dictionary<Vector3i, Sprite>();
            worldTilesforGroundTile = new World.Tile[tileWidth, tileHeight];
            bottomLeftTile = Vector3i.zero;
        }

		void Start() {
			InitializeGameObjects(MapView.Main.ViewRect);
			CrunchManager.AddRoutine(()=>PrecrunchTileTextures(), Constants.PlayerControlInitialized);
        }

		void Update() {
            UpdateTiles(MapView.Main.ViewRect);
        }

		IEnumerator PrecrunchTileTextures() {
            var mid = GetTilePosition(PlayerControl.Position);
            for(int x=mid.x-precrunchRadius*tileWidth;
				x<=mid.x+precrunchRadius*tileWidth;
				x+=tileWidth) {

				for(int y=mid.y-precrunchRadius*tileHeight;
					y<=mid.y+precrunchRadius*tileHeight;
					y+=tileHeight) {

                    GetSprite(new Vector2(x, y));
                    yield return 1;

                }

			}
        }

		public void InitializeGameObjects(Rect tileRect) {
			visibleTilesWidth = (int)(tileRect.width / tileWidth) + 1;
            visibleTilesHeight = (int)(tileRect.height / tileHeight) + 1;
            currentTiles = new Tile[visibleTilesWidth, visibleTilesHeight];
            for(int x=0;x<visibleTilesWidth;x++) {
				for(int y=0;y<visibleTilesHeight;y++) {
                    var newTileGO = new GameObject();
					newTileGO.transform.OrientTo(transform);
                    newTileGO.AddComponent<SpriteRenderer>();
                    var newTile = newTileGO.AddComponent<Tile>();
                    currentTiles[x, y] = newTile;
                }
			}
        }

		void UpdateTiles(Rect tileRect) {
			int w = (int)(tileRect.width / tileWidth) + 1;
			int h = (int)(tileRect.height / tileHeight) + 1;
			Vector3i bl = GetTilePosition(tileRect.position);
			if(bl!=bottomLeftTile) {
                bottomLeftTile = bl;
                RefreshTiles();
			}
            Vector3 offset = PlayerControl.Position;
            for(int x=0;x<w;x++) {
				for(int y=0;y<h;y++) {
                    var tile = currentTiles[x, y];
					tile.transform.localPosition = tile.WorldPosition - offset;
                }
			}
		}

		Sprite GetSprite(Vector2 position) {
            Sprite sprite;
            var lookupPosition = GetTilePosition(position);
			var basePos = (Vector2)lookupPosition;

            if(!spriteLookup.TryGetValue(lookupPosition, out sprite)) {
				Debugx.Tick();
                groundTextureGenerator.Position = basePos;
                var tex = groundTextureGenerator.Generate();
				tex.filterMode = FilterMode.Point;

                sprite = Sprite.Create(
                    tex,
                    TilePixelRect,
                    Vector2.zero,
                    Constants.SpritePPU);
                spriteLookup.Add(lookupPosition, sprite);
                generatedTileCount = spriteLookup.Count;
				Debugx.Tock();				
            }
			return sprite;
		}

		Vector3i GetTilePosition(Vector2 position) {
			return new Vector3i(
                (int)(position.x / tileWidth) * tileWidth,
                (int)(position.y / tileHeight) * tileHeight,
				0
            );
		}

		void RefreshTiles() {
            for (int x = 0; x < visibleTilesWidth; x++)
            {
                for (int y = 0; y < visibleTilesHeight; y++)
                {
                    var tile = currentTiles[x, y];
                    tile.WorldPosition = bottomLeftTile + new Vector3i(x * tileWidth, y * tileHeight, 0);
                    World.Map.GetTiles(
                        new Rect(tile.WorldPosition.x, tile.WorldPosition.y, tileWidth, tileHeight),
                        ref worldTilesforGroundTile
                    );
                    tile.SetTiles(worldTilesforGroundTile);
                    tile.SetSprite(GetSprite(tile.WorldPosition));
                }
            }
        }
    }
}
