using UnityEngine;
using System.Collections;

namespace Audio {
	public class Envelope : IAudioFilter {

		public AnimationCurve envelope = new AnimationCurve();

		private Player player;

		public void Init(Player player) { this.player = player; }
		public void Reset(){}
		public float Filter(float value) {
			return value*envelope.Evaluate(player.ElapsedNormalized);
		}
	}
}
