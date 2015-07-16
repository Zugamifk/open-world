using UnityEngine;
using System.Collections;

namespace Audio {
	public interface IAudioGenerator {
		void Init(float samplerate);
		void Generate(float[] __data, int __channels);
	}
}
