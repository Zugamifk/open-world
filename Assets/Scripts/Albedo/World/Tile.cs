using UnityEngine;
using System.Collections;
using Extensions;

namespace Albedo.World {
	public class Tile {
        public Vector3i position;

        public void Draw(Vector2 origin) {
			// Debugx.DrawCross(position-origin, 0.1f, new ColorHSV(position.x/5f, Math.USin(position.y/5), 0.8f));
		}
    }
}
