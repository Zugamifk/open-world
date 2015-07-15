/*____________________________________________________________________________

FPSCounter.cs
Displays the current framerate.
____________________________________________________________________________*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text)) ]
[AddComponentMenu("Utilities/FPS Counter")]
public class FPSCounter : MonoBehaviour {

	private float[] times = new float[15];
	private int p;

	private Text fpsText;
	private void Start(){
		fpsText = GetComponent<Text> ();
	}

	private void Update() {
		if (fpsText.enabled) {

			times [p] = Time.deltaTime;

			p = (p + 1) % times.Length;

			if (p != times.Length - 1) {
				return;
			}

			float sum = 0;
			for (int i = 0; i < times.Length; i++) {
				sum += times [i];
			}

			fpsText.text = Mathf.Round (1f / (sum / times.Length)) + "fps";
		} else {
			fpsText.text = "";
		}
	}
}
