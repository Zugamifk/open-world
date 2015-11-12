using UnityEngine;
using System.Collections;

namespace Albedo.World {
	public class Map : MonoBehaviour {

        protected Tiling tiles;

        private static Map instance;

		public static Transform Transform {
			get {
                return instance.transform;
            }
		}

		public static Vector2 Middle {
			get {
                return new Vector2(instance.tiles.Tiles.GetLength(0) / 2,
                                    instance.tiles.Tiles.GetLength(1) / 2
                                    );
            }
		}

		public static void GetTiles(Rect rect, ref Tile[,] tiles) {
            var X = (int)rect.width;
            var Y = (int)rect.height;
            if(	tiles.GetLength(0) < X ||
				tiles.GetLength(1) < Y ) {
                return;
            }

            var rx = (int)rect.x;
            var ry = (int)rect.y;
            for(int x=0;x<X;x++) {
				for(int y=0;y<Y;y++) {
                    tiles[x, y] = instance.tiles.Tiles[rx + x, ry + y];
                }
			}
		}

		public static Tile GetTile(Vector3i position) {
            return instance.tiles.Tiles[position.x, position.y];
        }

        void Awake() {
            this.SetInstanceOrKill(ref instance);
            tiles = new Tiling();
        }
    }
}
