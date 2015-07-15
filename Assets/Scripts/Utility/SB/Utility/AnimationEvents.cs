/*____________________________________________________________________________

AnimationEvents.cs


____________________________________________________________________________*/

using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour {
	
	public AudioClip[] m_audioClips;
	public bool singleton;

	public static bool SingletonRunning = false;
	
	private void PlayRandomClip(){
		if (singleton) {
			if (SingletonRunning) return;
			SingletonRunning = true;
		}

		var clip = m_audioClips[Random.Range(0,m_audioClips.Length)];
		AudioSource.PlayClipAtPoint(clip, transform.position);

		if (singleton) StartCoroutine(GenericAnimators.DelayedAnimation(clip.length, ()=>AnimationEvents.SingletonRunning = false));
	}
	
	private void PlayClipByName(string name){
		for (int i = 0; i < m_audioClips.Length; i++) {
			if(m_audioClips[i].name == name){
				AudioSource.PlayClipAtPoint(m_audioClips[i], transform.position);
			}
		}
	}

	public ParticleSystem[] m_particleSystems;
		
	private void PlayParticleSystem(AnimationEvent ae){
		foreach(ParticleSystem ps in m_particleSystems){
			if(ps.name == ae.stringParameter){
				ps.Play();
			}
		}
	}
	
	private void PlayAllParticleSystems(AnimationEvent ae){
		foreach(ParticleSystem ps in m_particleSystems){
			ps.Play();
		}
	}

}
