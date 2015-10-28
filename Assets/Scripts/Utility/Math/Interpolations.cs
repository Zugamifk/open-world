/*
	A bunch of functions related to interpolating, tweening, easing, smoothing, etc.
*/

using UnityEngine;
using System;
using System.Collections;

namespace Extensions {
	public static class Interpolation {

		public static float Const0(float _)
		{
            return 0;
        }

        public static float Const1(float _)
        {
            return 1;
        }

        public static Func<float,float> ConstN(float n) {
			return _=>n;
		}

		public static float Linear(float n) {
			return n;
		}

		public static float Sine(float t) {
            return 1-Math.UNCos(t * 0.5f);
        }

	}
}
