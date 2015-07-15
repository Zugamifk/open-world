using UnityEngine;
using System.Collections;

namespace Audio {
	public interface IAudioGenerator {
		void Generate(float[] __data, int __channels);
	}
}
