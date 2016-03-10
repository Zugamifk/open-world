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

        public static int Mod(int num, int mod) {
            var res = num%mod;
            return num < 0 ? res+mod : res;
        }

        public static float Place(float min, float max, float num) {
            return (num-min)/(max-min);
        }

        public static bool Between(float num, float low, float high) {
            return num > low && num < high;
        }

        public static int RoundUpMagnitude(float f)
        {
            if (f > 0)
            {
                return Mathf.CeilToInt(f);
            }
            else if (f < 0)
            {
                return Mathf.FloorToInt(f);
            }
            else return 0;
        }

        /** Interval on 0-1 rather than 0-2pi */
        public static float USin(float t) {
            return Mathf.Sin(t*2*Mathf.PI);
        }

        /** Interval on 0-1 rather than 0-2pi */
        public static float Cos(float t) {
            return Mathf.Cos(t*2*Mathf.PI);
        }

        /** Normalized Sine */
        public static float NSin(float t) {
            return 0.5f+0.5f*Mathf.Sin(t);
        }

        /** Normalized Cosine */
        public static float NCos(float t) {
            return 0.5f+0.5f*Mathf.Cos(t);
        }

        /** Normalized Sine with interval on 0-1 rather than 0-2pi */
        public static float UNSin(float t) {
            return 0.5f+0.5f* Mathf.Sin(t*2*Mathf.PI);
        }

        /** Normalized Cosine on interval on 0-1 rather than 0-2pi */
        public static float UNCos(float t) {
            return 0.5f+0.5f*Mathf.Cos(t*2*Mathf.PI);
        }

        public static int IntPow(int x, int pow)
        {
            int ret = 1;
            while ( pow != 0 )
            {
                if ( (pow & 1) == 1 )
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        public static float Repeat(float t, float length) {
            if(t<0) {
                return length - Mathf.Repeat(-t, length);
            } else {
                return Mathf.Repeat(t, length);
            }
        }

    }

}
