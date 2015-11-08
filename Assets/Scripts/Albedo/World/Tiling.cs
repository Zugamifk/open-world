using UnityEngine;
using System.Collections;

namespace Albedo.World {
	public class Tiling {
        protected Tile[,] tiles;

		public Tile[,] Tiles {
			get {
				return tiles;
			}
		}

		public Tiling() {
            tiles = new Tile[1024, 1024];
			for(int x=0;x<1024;x++) {
				for(int y=0;y<1024;y++) {
                    var tile = new Tile();
                    tile.position = new Vector3i(x, y,0);
                    tiles[x, y] = tile;
                }
			}
        }
    }
}
