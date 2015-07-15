/*____________________________________________________________________________

SlaveCamera

____________________________________________________________________________*/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SlaveCamera : MonoBehaviour {

	[SerializeField] Camera m_master;

	protected void LateUpdate() {
		camera.orthographicSize = m_master.orthographicSize;
	}

}
