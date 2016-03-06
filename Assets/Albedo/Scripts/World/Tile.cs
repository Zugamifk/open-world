using UnityEngine;
using System.Collections;
using Extensions;

namespace Albedo.World {
	public class Tile {
        public Vector3i position;
        public TileObject[] objects;
        public int objectCount;

        public bool Collides {
            get;
            protected set;
        }

        public bool Visible;

        public Tile() {
            objects = new TileObject[Constants.MaxTileObjects];
            objectCount = 0;
			if(Random.value<0.03f) {
                var to = new TileObject();
				to.tile = this;
                to.name = "Test";

                objects[0] = to;
                objectCount++;
				Collides = true;
            }
        }

        public void Draw(Vector2 origin) {
			// Debugx.DrawCross(position-origin, 0.1f, new ColorHSV(position.x/5f, Math.USin(position.y/5), 0.8f));
		}

		public static Tile EmptyTile() {
            return new Tile();
        }
    }
}
