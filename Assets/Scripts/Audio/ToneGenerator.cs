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

		public void Init(Player p) {
		}

		public float Generate(float time) {
			 Signal signal = new Signal(Math.Const0);
			 switch(type) {
				 case ToneType.NONE: break;
				 case ToneType.SINE: signal = Math.USin; break;
				 case ToneType.SQUARE: signal = t => (t % 1 > 0.5f) ? 1 : -1; break;
				 case ToneType.TRIANGLE: signal = t => (t*2>1) ? 3-4*t : 4*t-1; break;
			 }

			 return signal(time*frequency);
		}
	}
}
