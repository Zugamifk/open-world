using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {
	using World = Albedo.World;
	public class TileAnimator : MonoBehaviour {

		protected World.Tile tile;
		protected Vector3i worldPosition;

        protected TileObjectAnimator[] objects;
        protected int objectCount;

		public World.Tile Tile {
			get {
				return tile;
			}
			set {
				tile = value;
			}
		}

		public Vector3i WorldPosition {
			get{
                return worldPosition;
            }
			set {
				worldPosition = value;
			}
		}

		void Awake() {
            objects = new TileObjectAnimator[Constants.MaxTileObjects];
			for(int i=0;i<Constants.MaxTileObjects;i++) {
                var go = new GameObject();
				go.transform.OrientTo(transform);
                go.name = string.Format("object {0}", i);
                var obj = go.AddComponent<TileObjectAnimator>();
				objects[i] = obj;
            }
            objectCount = 0;
            name = string.Format("Tile [{0},{1}]", WorldPosition.x, WorldPosition.y);
        }

		void Update() {
            System.Action<TileAnimator> debugdraw;
            if(MapView.DebugTiles.TryGetValue(WorldPosition, out debugdraw)) {
                debugdraw(this);
            }
		}

		public void DrawOutline(Color color) {
			Debugx.DrawRect(new Rect(transform.position.x, transform.position.y, 1, 1), color);
		}

        public void SetTile(World.Tile tile) {
			this.tile = tile;
			worldPosition = tile.position;
			name = string.Format("Tile [{0},{1}]", WorldPosition.x, WorldPosition.y);
            int i = 0;
            int oldObjectCount = objectCount;
            for(;i<tile.objectCount;i++) {
				objects[i].SetObject(tile.objects[i]);
			}
			for(;i<oldObjectCount;i++) {
				objects[i].SetObject(null);
			}
            objectCount = tile.objectCount;
        }

	}
}
