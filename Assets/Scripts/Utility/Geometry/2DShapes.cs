using UnityEngine;
using System.Collections;
using Extensions;
using System.Collections.Generic;

namespace Geometry {
	public static partial class Shapes {

		public static Polygon Monogon() {
			return new Polygon(Vector2.zero);
		}

		public static Polygon Square() {
			return new Polygon(
				new Vector2[]{
					Vector2.zero, Vector2.right, new Vector2(1,1), Vector2.up
				}
			);
		}

	}
}
