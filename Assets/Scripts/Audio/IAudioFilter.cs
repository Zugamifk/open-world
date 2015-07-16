using UnityEngine;
using System.Collections;

namespace Audio {
	public interface IAudioFilter {
		void Init(Player player);
		void Reset();
		float Filter(float time);
	}
}
