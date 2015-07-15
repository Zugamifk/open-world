using UnityEngine;
using System.Collections;

public static class Colorx {

    private const int NumPrimitiveColors = 9;
    public static Color[] AllColors = {
		Color.black,
		Color.blue,
		Color.cyan,
		Color.gray,
		Color.green,
		Color.magenta,
		Color.red,
		Color.white,
		Color.yellow,
		pink,
		orange,
		wine,
		butterflyBlue,
		darkTeal,
		megaPink,
		clay,
		moon,
		burgundy
	};

	public static Color pink {
		get { return new Color(1,0.6f,0.6f,1); }
	}

	public static Color orange {
		get { return FromHex(250,92,30); }
	}

	public static Color wine {
		get { return FromHex(116,1,64); }
	}

	public static Color butterflyBlue {
		get { return FromHex (0x06, 0xA5, 0xDC); }
	}

	public static Color darkTeal {
		get { return FromHex(0x4F, 0x86, 0x99); }
	}

	public static Color megaPink {
		get { return FromHex(0xFF, 0x23, 0x6B); }
	}

	public static Color clay {
		get { return FromHex(0xFF4E57); }
	}

	public static Color moon {
        get { return FromHex(0xEEE6AB); }
     }

	public static Color burgundy {
        get { return FromHex(0x881E3F); }
     }

    public static Color popperGreen {
        get { return FromHex(0x0CA207); }
    }

	public static Color RandomPrimitive {
		get {
			return AllColors[Random.Range(0, NumPrimitiveColors)];
		}
	}

	public static Color RandomPrimitivePlus {
		get {
			return AllColors[Random.Range(0, AllColors.Length-1)];
		}
	}

	public static Color RandomAny {
		get {
			return new Color(Random.value, Random.value, Random.value);
		}
	}

    public static Color RandomHue(Color c) {
        var hsv = (ColorHSV)c;
        hsv.h = Random.value;
        return hsv;
    }

	public static Color FromHex(int r, int g, int b) {
		float cr = (float)r;
		float cg = (float)g;
		float cb = (float)b;
		return new Color(cr/255f,cg/255f,cb/255f);
	}

	public static Color FromHex(int hex) {
		float cr = (float)(hex/0xffff);
		float cg = (float)((hex/0xff)%0xff);
		float cb = (float)(hex%0xff);
		return new Color(cr/255f,cg/255f,cb/255f);
	}

	public static Color SetAlpha(this Color c, float a) {
		var col = c;
		col.a = a;
		return col;
	}

    public static Color SetRGB(this Color c, Color rgb) {
        var col = c;
        col.r = rgb.r;
        col.g = rgb.g;
        col.b = rgb.b;
        return col;
    }

	public static float Hue(this Color c) {
		float m=Mathf.Min (c.r, c.g, c.b);
		if (c.r > c.g && c.r>c.b) {
			return 60 * (((c.g-c.b)/(c.r-m))%6) ;
		} else if(c.g>c.r && c.g>c.b) {
			return 60 * ((c.b-c.r)/(c.g-m)+2);
		} else if(c.b>c.r && c.b>c.g) {
			return 60 * ((c.r-c.g)/(c.b-m)+4);
		} else return 0;
	}

	public static float Saturation(this Color c) {
		var alpha = (2*c.r - c.g - c.b)/2;
		var beta = (c.g-c.b)*Mathf.Sqrt(3)/2;
		return Mathf.Sqrt (alpha*alpha+beta*beta)/c.Value ();
	}

	public static float Value(this Color c) {
		return Mathf.Max(c.r, c.g, c.b);
	}

	public static Vector3 RGBtoHSV(this Color c) {
		return new Vector3(
			c.Hue(),
			c.Saturation(),
			c.Value());
	}

	public static Color HSVtoRGB(Vector3 HSV) {
		Color col = Color.black;
		var C = HSV.z * HSV.y;
		var H = HSV.x/60;
		var X = C*(1-Mathf.Abs (H%2-1));
		// Debug.Log (HSV);
		switch(Mathf.FloorToInt(H)) {
		case 0: col = new Color(C,X,0); break;
		case 1: col = new Color(X,C,0); break;
		case 2: col = new Color(0,C,X); break;
		case 3: col = new Color(0,X,C); break;
		case 4: col = new Color(C,0,X); break;
		case 5: col = new Color(X,0,C); break;
		}
		var m = HSV.z-C;
		col.r+=m;
		col.g+=m;
		col.b+=m;
		return col;
	}

	public static Color SetHue(this Color c, float h) {
		var hsv = c.RGBtoHSV();
		hsv.x = h;
		var col = Colorx.HSVtoRGB(hsv);
		col.a = c.a;
		return col;
	}

	public static Color SetSaturation(this Color c, float s) {
		var hsv = c.RGBtoHSV();
		hsv.y = s;
		var col = Colorx.HSVtoRGB(hsv);
		col.a = c.a;
		return col;
	}

	public static Color SetValue(this Color c, float v) {
		v = Mathf.Clamp01(v);
		var scale = v/Mathf.Max(c.r, c.g, c.b);
		c.r *= scale;
		c.g *= scale;
		c.b *= scale;
		return c;
	}
}
