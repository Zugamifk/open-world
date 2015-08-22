using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

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

	public static Color lightmaroon {
		get {
			return FromHex(0x853C43);
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

	public static IEnumerable<Color> FibonacciHues(Color seed) {
		var col = (ColorHSV)seed;
		while(true) {
			yield return col;
			col.h = (col.h+Math.InversePhi)%1;
		}
 	}
}
