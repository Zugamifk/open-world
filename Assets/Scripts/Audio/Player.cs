using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Audio {
	[ExecuteInEditMode]
	[RequireComponent(typeof(AudioSource))]
	public class Player : MonoBehaviour {

		public IAudioGenerator generator;
		public float time;
		public List<IAudioFilter> filters = new List<IAudioFilter>();

		private AudioSource audio;
		private float elapsed;
		private const float samplerate = 44000;

		public float Elapsed {
			get { return elapsed; }
		}

		public float ElapsedNormalized {
			get { return elapsed/time; }
		}

		// Use this for initialization
		void Start () {
			audio = GetComponent<AudioSource>();
			audio.clip = new AudioClip();
			audio.volume = 1f;
			audio.pitch = 1f;
			audio.priority = 128;
			Reset();
		}

		public void Reset() {
			elapsed = 0;
			filters.ForEach(f=>f.Init(this));
			filters.ForEach(f=>f.Reset());
		}

		public void Play() {
			Reset();
			generator.Init(this);
			audio.Play();
		}

		void OnAudioFilterRead(float[] data, int channels)
		{
			if (generator!=null && elapsed < time) {
				int numSamples = data.Length / channels;
				float dt = 1f/samplerate;
				for(int i = 0; i< numSamples; i++) {
					for(int c = 0;c<channels;c++) {
						data[i*channels + c] = filters
							.Aggregate(
								generator.Generate(elapsed),
								(sample, filter) => filter.Filter(sample)
							);
					}
				   elapsed += dt;
				}
			}
		}
	}
}
