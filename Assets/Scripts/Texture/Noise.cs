using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Extensions;

namespace TextureGenerator {
	public class Noise : TextureGenerator {
        public Gradient colors;
        public AnimationCurve FrequencyDistribution;
        public AnimationCurve AmplitudeDistribution;
        public int MinimumFrequency = 1;
		public int MaximumFrequency = 1;
        public int Steps = 1;

        public Math.Noise2D sampler;

		public override IEnumerable<Color> GetPixels() {
            sampler = Math.FrequencyNoise2D(
				Interpolation.Sine,
				t=>t,
				x=>AmplitudeDistribution.Evaluate(x),
				MinimumFrequency,
				MaximumFrequency,
				Steps
			);

            float xs = 1f/(float)width;
			float ys = 1f/(float)height;

            float xi = 0;
            float yi = 0;

            for (int y = 0; y < height; y++)
            {
                xi = 0;
                for (int x = 0; x < width; x++)
                {
                    yield return Color.white * sampler(xi, yi);
                    xi += xs;
                }
                yi += ys;
            }
        }
	}
}
