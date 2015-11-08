using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {
	using World = Albedo.World;
	public class TileAnimator : MonoBehaviour {

		protected World.Tile tile;
		protected Vector3i worldPosition;

        protected World.TileObject[] objects;
        protected int objectCount;

        public void SetTile(World.Tile tile) {
			this.tile = tile;
		}

	}
}
