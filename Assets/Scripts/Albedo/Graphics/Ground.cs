using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Textures;
using Albedo;

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

        protected GroundTexture groundTextureGenerator;
        protected Dictionary<Vector3i, Sprite> spriteLookup;
        protected Tile[,] currentTiles;

        public class Tile : MonoBehaviour {
            protected World.Tile[,] contained;
            new protected Renderer renderer;
            protected Vector3i worldPosition;

			public Vector3i WorldPosition {
				get {
                    name = string.Format("Ground Tile [{0}, {1}]", worldPosition.x, worldPosition.y);
                    return worldPosition;
				}
				set{
					worldPosition = value;
				}
			}

            void Awake() {
                renderer = GetComponent<Renderer>();
            }

			public void SetTiles(World.Tile[,] tiles) {
				contained = tiles;
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
        }

		void Start() {
			InitializeGameObjects(MapView.Main.ViewRect);
		}

		void Update() {
            UpdateTiles(MapView.Main.ViewRect);
        }

		public void InitializeGameObjects(Rect tileRect) {
			int w = (int)(tileRect.width / tileWidth) + 1;
            int h = (int)(tileRect.height / tileHeight) + 1;
            currentTiles = new Tile[w, h];
            for(int x=0;x<w;x++) {
				for(int y=0;y<h;y++) {
                    var newTileGO = new GameObject();
					newTileGO.transform.OrientTo(transform);
                    var ren = newTileGO.AddComponent<SpriteRenderer>();
                    ren.sprite = GetSprite(new Vector2(x*tileWidth,y*tileHeight));
                    var newTile = newTileGO.AddComponent<Tile>();
                    currentTiles[x, y] = newTile;
                }
			}
        }

		void UpdateTiles(Rect tileRect) {
			int w = (int)(tileRect.width / tileWidth) + 1;
			int h = (int)(tileRect.height / tileHeight) + 1;
			Vector3i origin = new Vector3i(tileRect.x, tileRect.y, 0);
            Vector3 offset = PlayerControl.Position;
            for(int x=0;x<w;x++) {
				for(int y=0;y<h;y++) {
                    var tile = currentTiles[x, y];
                    tile.WorldPosition = origin + new Vector3i(x*tileWidth, y*tileHeight, 0);
                    var worldTiles = new World.Tile[tileWidth, tileHeight];
					World.Map.GetTiles(
						new Rect(tile.WorldPosition.x, tile.WorldPosition.y, tileWidth, tileHeight),
						ref worldTiles
					);
					tile.SetTiles(worldTiles);
					tile.transform.localPosition = tile.WorldPosition - offset;
                }
			}
		}

		Sprite GetSprite(Vector2 position) {
            Sprite sprite;
            Vector2 basePos = new Vector2(
                (int)(position.x / tileWidth) * tileWidth,
                (int)(position.y / tileHeight) * tileHeight
            );
            var lookupPosition = (Vector3i)basePos;
            if(!spriteLookup.TryGetValue(lookupPosition, out sprite)) {
                groundTextureGenerator.Position = basePos;
                var tex = groundTextureGenerator.Generate();
                tex.filterMode = FilterMode.Point;
                sprite = Sprite.Create(
                    tex,
                    TilePixelRect,
                    Vector2.zero,
                    Constants.SpritePPU);
                spriteLookup.Add(lookupPosition, sprite);
            }
			return sprite;
		}
    }
}
