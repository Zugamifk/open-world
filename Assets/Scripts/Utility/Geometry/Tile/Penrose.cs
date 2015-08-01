// PENROSE TILES

﻿using UnityEngine;
using System.Collections;
using Extensions;
using System.Collections.Generic;

namespace Geometry {
	public static partial class Shapes {
		public static Polygon Kite() {
			var pt = Vector2.zero;
			var dir = Vector2.right;
			float len = Math.Phi;

			var verts = new List<Vector2>();
			verts.Add(pt);

			pt += dir * len;
			verts.Add(pt);

			dir = (-dir).Rotate(-72);
			len = 1;
			pt += dir * len;
			verts.Add(pt);

			dir = (-dir).Rotate(-144);
			pt += dir * len;
			verts.Add(pt);

			return new Polygon(
				verts.ToArray()
			);
		}

		public static Polygon Dart() {
			var pt = Vector2.zero;
			var dir = Vector2.right.Rotate(-72);
			float len = 1;

			var verts = new List<Vector2>();
			verts.Add(pt);

			pt += dir * len;
			verts.Add(pt);

			dir = (-dir).Rotate(-36);
			len = Math.Phi;
			pt += dir * len;
			verts.Add(pt);

			dir = (-dir).Rotate(-72);
			pt += dir * len;
			verts.Add(pt);

			return new Polygon(
				verts.ToArray()
			);
		}
	}
}
