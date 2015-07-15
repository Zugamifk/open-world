using UnityEngine;
using System.Collections;

public class RainSplash : MonoBehaviour {

	[SerializeField] protected float m_splashRate;
	[SerializeField] protected float m_splashTime;
	[SerializeField] protected Texture[] m_splashFrames;

	void Awake() {
		renderer.material.mainTexture = m_splashFrames[0];
	}

	// Update is called once per frame
	void Update () {
		float t = Time.time % m_splashRate;
		if (t < m_splashTime ) {
		    float f = 0.0f;
			float fl = m_splashTime/(float)m_splashFrames.Length;
		    for (int i = 0; i < m_splashFrames.Length; i++) {
				if (t > f && t < f+fl) {
					renderer.material.mainTexture = m_splashFrames[i];
					if (i == 0 && !renderer.enabled) {
						renderer.material.SetTextureOffset("_MainTex", new Vector2(Random.value, 0));
						renderer.enabled = true;
					}
					break;
				}
				f += fl;
			}
		} else if (t > m_splashTime && renderer.enabled) {
			renderer.enabled = false;
		}
	}
}
