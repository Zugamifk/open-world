using UnityEngine;
using System.Collections;

public struct ColorHSV {
	public float h, s, v;
	public ColorHSV(float h, float s, float v) {
		this.h = h;
		this.s = s;
		this.v = v;
	}
	public static ColorHSV Lerp(ColorHSV a, ColorHSV b, float t) {
		return new ColorHSV(
			Mathf.Lerp(a.h, b.h, t),
			Mathf.Lerp(a.s, b.s, t),
			Mathf.Lerp(a.v, b.v, t)
		);
	}

	public override string ToString() {
        return string.Format("ColorHSV ({0}, {1}, {2})", h, s, v);
    }
	public static implicit operator Color(ColorHSV hsv) {
		float h = hsv.h;
		float s = hsv.s;
		float v = hsv.v;

		if(Mathf.Approximately(s, 0)) {
			return Color.white*v;
		}

		h = Extensions.Math.Repeat(h,1) * 6;
		int i = Mathf.FloorToInt(h);
		float f = h - i;
		float p = v * (1 - s);
		float q = v * (1 - s * f);
		float t = v * (1 - s * (1 - f));

		switch(i) {
			case 0: return new Color(v, t, p);
			case 1: return new Color(q, v, p);
			case 2: return new Color(p, v, t);
			case 3: return new Color(p, q, v);
			case 4: return new Color(t, p, v);
			case 5: return new Color(v, p, q);
		}

		return Color.white;
	}
	public static explicit operator ColorHSV(Color col) {
		float r = col.r;
		float g = col.g;
		float b = col.b;
		float min, max, step;
		min = Mathf.Min(r, g, b);
		max = Mathf.Max(r, g, b);
		step = max - min;

		if(Mathf.Approximately(max, 0)) {
			return new ColorHSV(0,0,0);
		}

		float hue = 0;
		if(Mathf.Approximately(r, max)) {
			hue = (g-b)/step;
		} else
		if(Mathf.Approximately(g, max)) {
			hue = 2 + (b-r)/step;
		} else {
			hue = 4 + (r-g)/step;
		}
		hue /=6;
		if(hue<0) hue += 1;

		return new ColorHSV(hue, step / max, max);
	}
}
