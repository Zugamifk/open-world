using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Textures {
	public abstract class TextureGenerator {
        public int width = 256;
        public int height = 256;
        public string name = "NONE";

		public string Name {
			get {
                return name;
            }
		}

		public abstract IEnumerable<Color> GetPixels();
        public virtual Texture2D Generate() {
            var tex = new Texture2D(width, height);
            var pixels = GetPixels().ToArray();
            tex.SetPixels(pixels);
			tex.name = name;
            tex.Apply();
			return tex;
        }
    }
}
