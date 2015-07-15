using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InputObserver : MonoBehaviour {
    public delegate void InputEvent();
	public InputEvent OnTap {
		get; set;
    }

	void OnGUIInput() {
        if(OnTap!=null) OnTap();
    }
}
