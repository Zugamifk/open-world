using UnityEngine;
using System.Collections;
using System.Linq;
using Extensions;

namespace Extensions {
	public static partial class Math {
        public delegate float Noise1D(float x);
        public delegate float Noise2D(float x, float y);

		private static HashFunction GetHasher() {
            return new WangDoubleHash(Random.Range(0, int.MaxValue));
        }

		public static Noise2D Noise1Dto2D(Noise1D generator) {
            return (x, y) => 0.5f * (generator(x) + generator(y));
        }

        public static float UniformNoise(float x)
        {
            return Random.value;
        }

		public static Noise1D FrequencyNoise1D(
			System.Func<float, float> signal,
			System.Func<float, float> frequencyDistribution,
			System.Func<float, float> amplitudeDistribution,
			float minFreq,
			float maxFreq,
			int steps
		) {
			float step = steps>1 ? 1/(float)steps : 1;
            HashFunction[] hashers = Enumerable.Range(0, steps).Select(_ => GetHasher()).ToArray();

            return x => {
                float val = 0;
                float t = 0;
                float normalizer = 0;
                for (int i=0;i<steps;i++) {
                    val += signal(hashers[i].Value((int)(x*Mathf.Lerp(minFreq, maxFreq, frequencyDistribution(t))))) * amplitudeDistribution(t);
					normalizer += amplitudeDistribution(t);
					t += step;
                }
				return val/normalizer;
			};
        }

		public static Noise2D FrequencyNoise2D(
			System.Func<float, float> signal,
			System.Func<float, float> frequencyDistribution,
			System.Func<float, float> amplitudeDistribution,
			float minFreq,
			float maxFreq,
			int steps
		) {
			float step = steps>1 ? 1/(float)steps : 1;

            HashFunction[] hashers = Enumerable.Range(0, steps).Select(_ => GetHasher()).ToArray();

            System.Func<float, float, float, int, float> valueInSquare = (x, y, f, i) =>
            {
                int l = (int)(x * f);
                int r = l + 1;
                int b = (int)(y * f);
                int t = b + 1;
                float xt = Math.Repeat(x * f, 1);
                float yt = Math.Repeat(y * f, 1);
                return Mathf.Lerp(
                    Mathf.Lerp(hashers[i].Value(l, b), hashers[i].Value(r, b), signal(xt)),
                    Mathf.Lerp(hashers[i].Value(l, t), hashers[i].Value(r, t), signal(xt)),
                    signal(yt)
                );
            };


            return (x,y) => {
                float val = 0;
                float t = 0;
                float normalizer = 0;
                float freq = 0;
                for (int i=0;i<steps;i++) {
					freq = Mathf.Lerp(minFreq, maxFreq, frequencyDistribution(t));
                	val += valueInSquare(x,y,freq,i) * amplitudeDistribution(t);

					normalizer += amplitudeDistribution(t);
					t += step;
                }
				return val/normalizer;
			};
        }
    }
}
