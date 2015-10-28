using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Extensions;

namespace Textures {
	public enum NoiseTexturePresets {
		None,
		White,
		Pink,
		Red,
		Blue,
		Violet,
		Perlin_Sine,
		Perlin_Discrete,
		Perlin_Linear
	}

	public class Noise : TextureGenerator {
        public NoiseTexturePresets preset;
        public AnimationCurve InterpolationCurve;
        public AnimationCurve FrequencyDistributionCurve;
        public AnimationCurve AmplitudeDistributionCurve;
        public float MinimumFrequency = 1;
		public float MaximumFrequency = 1;
        public int Steps = 1;

        protected System.Func<float, float> interpolation;
		protected System.Func<float, float> frequencyDistribution;
        protected System.Func<float, float> amplitudeDistribution;

        protected Math.Noise2D sampler;

		protected void GetPreset() {
			switch(preset) {
				default:
				case NoiseTexturePresets.None: {
                    interpolation = t=>InterpolationCurve.Evaluate(t);
                    frequencyDistribution = t=>FrequencyDistributionCurve.Evaluate(t);
					amplitudeDistribution = t=>AmplitudeDistributionCurve.Evaluate(t);
                } break;
				case NoiseTexturePresets.White: {
                    frequencyDistribution = Interpolation.Linear;
                    amplitudeDistribution = Interpolation.Const1;
                    interpolation = Interpolation.Const0;
                } break;
				case NoiseTexturePresets.Perlin_Sine: {
                    interpolation = Interpolation.Sine;
                    frequencyDistribution = t => Mathf.Pow(2, t) - 1;
                    amplitudeDistribution = t => Mathf.Pow(2, 1-t) - 1;
                } break;
            }
			sampler = Math.FrequencyNoise2D(
				interpolation,
				frequencyDistribution,
				amplitudeDistribution,
				MinimumFrequency,
				MaximumFrequency,
				Steps
			);
		}

		protected void RefreshSampler() {
			sampler = Math.FrequencyNoise2D(
				interpolation,
				frequencyDistribution,
				amplitudeDistribution,
				MinimumFrequency,
				MaximumFrequency,
				Steps
			);
		}

		public override IEnumerable<Color> GetPixels() {

            GetPreset();

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
