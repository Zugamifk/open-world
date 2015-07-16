using UnityEngine;
using System.Collections;
using Extensions;

namespace Audio {
	public enum ToneType{
		NONE, // silence
		SINE,
		SQUARE,
		TRIANGLE
	}
	public class ToneGenerator : IAudioGenerator {
		public ToneType type;
		public float frequency;

		private float sampleRate;
		private float time;

		public void Init(float rate) {
			sampleRate = rate;
			time = 0;
		}

		public void Init(ToneType type, float rate) {
			Init(rate);
			this.type = type;
		}

		public void Generate(float[] samples, int channels) {
			 Signal signal = new Signal(Math.Const0);
			 switch(type) {
				 case ToneType.NONE: break;
				 case ToneType.SINE: signal = Math.USin; break;
				 case ToneType.SQUARE: signal = t => (t % 1 > 0.5f) ? 1 : -1; break;
				 case ToneType.TRIANGLE: signal = t => (t*2>1) ? 3-4*t : 4*t-1; break;
			 }

			 int numSamples = samples.Length / channels;
			 float dt = frequency/sampleRate;
			 for(int i = 0; i< numSamples; i++) {
				 for(int c = 0;c<channels;c++) {
					 samples[i*channels + c] = signal(time);
				 }
				time += dt;
			 }

		}
	}
}
