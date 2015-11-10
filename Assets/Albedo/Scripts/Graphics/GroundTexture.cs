using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Textures;
using Albedo;

namespace Albedo.Graphics {
	public class GroundTexture : Noise {

        private Vector2 UnitScale;

        public Vector2 Position { get; set; }

        public Gradient Colors { get; set; }

        public GroundTexture(int unitWidth, int unitHeight) {
            GenerateMipMaps = false;
            AnisoLevel = 0;

            UnitScale = new Vector2(unitWidth, unitHeight);
            width = unitWidth * Constants.TileWidthPixels;
			height = unitHeight * Constants.TileHeightPixels;
            name = "Ground Tile";

            preset = NoiseTexturePresets.Perlin_Sine;
			GetPreset();

            MinimumFrequency = 0.25f;
			MaximumFrequency = width;
            Steps = Constants.TileWidthPixels;
            interpolation = t => t * t * t * (t * (t * 6 - 15) + 10);
			frequencyDistribution = t => t*t*t;

            RefreshSampler();
        }

		public override IEnumerable<Color> GetPixels() {

            float xs = UnitScale.x/(float)width;
			float ys = UnitScale.y/(float)height;

            float xi = Position.x;
            float yi = Position.y;

            for (int y = 0; y < height; y++)
            {
                xi = Position.x;
                for (int x = 0; x < width; x++)
                {
                    var t = sampler(xi, yi);
					t = 1-(1-t)*(1-t);
                    yield return Colors.Evaluate(t);
                    xi += xs;
                }
                yi += ys;
            }
        }
	}
}
