using UnityEngine;
using System.Collections;

namespace Extensions {
    public delegate float ParametricCurve(float t);
}

public static class Mathfx {

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

}
