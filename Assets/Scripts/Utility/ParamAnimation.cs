/*____________________________________________________________________________

ParamAnimation.cs

____________________________________________________________________________*/

using UnityEngine;
using System.Collections;

public class ParamAnimation : MonoBehaviour {

    //__________________________________________________/           Public \__

	public float Position {
		get{
			return m_position;
		}
		set{
			m_position = Mathf.Clamp01(value);
			GetComponent<Animation>().Play();
			GetComponent<Animation>()[GetComponent<Animation>().clip.name].time = Mathf.Clamp01(m_position) * GetComponent<Animation>().clip.length ;
			GetComponent<Animation>().Sample();
			GetComponent<Animation>().Stop();
		}
	}
	
    //__________________________________________________/        Protected \__

	private float m_position;	

}
