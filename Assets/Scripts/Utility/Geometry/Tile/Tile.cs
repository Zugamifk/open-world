using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geometry {
	public class Tile {
		public Polygon shape;

		public Tile(){}
		public Tile(Polygon s) {
			shape = s;
		}
	}
}
