/*
	A bunch of functions related to interpolating, tweening, easing, smoothing, etc.
*/

using UnityEngine;
using System.Collections;

namespace Extensions {
	public static class Interpolation {

		public static float Sine(float t) {
            return 1-Math.UNCos(t * 0.5f);
        }

	}
}
