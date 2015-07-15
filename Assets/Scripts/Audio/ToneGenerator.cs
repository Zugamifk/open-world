using UnityEngine;
using System.Collections;

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

		public void Generate(float[] samples, int channels) {
			
		}

	}
}
