using UnityEngine;
using System.Collections;

public static class Colorx {

	public static Color darkblue {
		get {
            return FromHex(0x0A0442);
        }
	}

	public static Color lightgreen {
		get {
            return FromHex(0x96FF8C);
        }
	}

	public static Color FromHex(int color) {
        var ints = new Color(
			color/0x10000,
			(color/0x100)%0x100,
			color%0x100,
			0x100
        );
        return ints / 255;
    }

	public static Color Shift(this Color col, float offset) {
		var value = (col.r+col.g+col.b)/3f;
		var newValue = value + offset;
		var ratio = newValue/value;
		return col*ratio;
	}

	public static Color TriadMix(Color a, Color b, Color c, float greyScale = 1) {
		switch(Random.Range(0,3)) {
			case 0: a*=greyScale; break;
			case 1: b*=greyScale; break;
			case 2: c*=greyScale; break;
        }
        return a * Random.value + b * Random.value + c * Random.value;
    }

	public static Color Complement(this Color col) {
        var hsv = (ColorHSV)col;
    	hsv.h = (hsv.h+0.5f)%1;
		return hsv;
	}

}

public struct ColorHSV {
	public float h, s, v;
	public ColorHSV(float h, float s, float v) {
		this.h = h;
		this.s = s;
		this.v = v;
	}
	public static implicit operator Color(ColorHSV hsv) {
		float h = hsv.h;
		float s = hsv.s;
		float v = hsv.v;

		if(Mathf.Approximately(s, 0)) {
			return Color.white*v;
		}

		h *= 6;
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
