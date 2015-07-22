using UnityEngine;
using System.Collections;

namespace Extensions {
    public delegate float ParametricCurve(float t);

    public static partial class Math {

        public const float Phi = 1.61803398875f;
        public const float InversePhi = 0.61803398875f;

        // _____________________________________/ General math tools \______________
        public static bool Approximately( Vector2 a, Vector2 b ) {
    		return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
    	}

    	public static float PerlinNoise(Vector2 p) {
    		return Mathf.PerlinNoise(p.x, p.y);
    	}

        public static float ClamplessLerp(float from, float to, float t) {
            return from + (to-from)*t;
        }

        public static float Mod(float num, float mod) {
            var res = num%mod;
            return num > 0 ? res : res + mod;
        }

        public static float Place(float min, float max, float num) {
            return (num-min)/(max-min);
        }

        /** Interval on 0-1 rather than 0-2pi */
        public static float USin(float t) {
            return Mathf.Sin(t*2*Mathf.PI);
        }

        /** Interval on 0-1 rather than 0-2pi */
        public static float UCos(float t) {
            return Mathf.Cos(t*2*Mathf.PI);
        }

    }

}
