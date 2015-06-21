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
}
