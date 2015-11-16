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

        protected TextureFormat format = TextureFormat.ARGB32;
        public TextureFormat Format {
			get {
				return format;
			}
			set {
				format = value;
			}
		}

		protected bool generateMipMaps = true;
		public bool GenerateMipMaps{
			get {
                return generateMipMaps;
            }
			set {
				generateMipMaps = value;
			}
		}

        protected int anisoLevel;
		public int AnisoLevel {
			get {
                return anisoLevel;
            }
			set {
				anisoLevel = value;
			}
		}

		public virtual Texture2D GetEmptyTexture() {
			var tex = new Texture2D(width, height, Format, GenerateMipMaps);
			tex.anisoLevel = anisoLevel;
			tex.name = name;
			return tex;
		}

		public virtual void Fill(Texture2D tex) {
			var pixels = GetPixels().ToArray();
			tex.SetPixels(pixels);
			tex.Apply();
		}

        public abstract IEnumerable<Color> GetPixels();
        public virtual Texture2D Generate() {
            var tex = GetEmptyTexture();
            Fill(tex);
            return tex;
        }
    }
}
