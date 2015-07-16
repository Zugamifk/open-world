using UnityEngine;
using System.Collections;

namespace Audio {
	public interface IAudioGenerator {
		void Init(Player p);
		float Generate(float time);
	}
}
