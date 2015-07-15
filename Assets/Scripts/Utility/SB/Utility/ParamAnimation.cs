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
			animation.Play();
			animation[animation.clip.name].time = Mathf.Clamp01(m_position) * animation.clip.length ;
			animation.Sample();
			animation.Stop();
		}
	}
	
    //__________________________________________________/        Protected \__

	private float m_position;	

}
