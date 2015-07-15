using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ColorPalette {
	public struct NamedColor {
		public string name;
		public Color color;
	}
    public List<Color> colors = new List<Color>();
    public List<string> names = new List<string>();

    public int size {
		get { return colors.Count; }
        set {
			int len = colors.Count;
            if (len > value)
            {
            	colors.RemoveRange(value, len-value);
				names.RemoveRange(value, len-value);;
			} else {
				for(int i=len;i<value;i++) {
					colors.Add(Color.black);
					names.Add("");
				}
			}
        }
    }
	public Color this[int i] {
        get { return colors[i]; }
		set {
			colors[i] = value;
		}
    }

	public Color this[string key] {
        get {
			for(int i=0;i<names.Count;i++) {
				if(names[i]==key) {
                    return colors[i];
                }
			}
			return Color.black;
        }
		set {
			for(int i=0;i<names.Count;i++) {
				if(names[i]==key) {
					colors[i] = value;
				}
			}
		}
    }
}
