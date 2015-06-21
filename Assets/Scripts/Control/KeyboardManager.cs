using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class KeyboardManager : MonoBehaviour {

    public delegate void KeyDownEvent();
    public Dictionary<KeyCode, KeyDownEvent> KeyEvents = new Dictionary<KeyCode, KeyDownEvent>();

	void Subscribe(KeyCode kc, KeyDownEvent e) {
		KeyEvents.Add(kc, e);
	}

	// Update is called once per frame
	void Update () {
		foreach(var kv in KeyEvents) {
			if(Input.GetKeyDown(kv.Key)&&kv.Value!=null)
                kv.Value();
        }
	}

	void Start() {
        var lines = "{0} : {1}".EnumerableFormat(Enumerable.Range(2, 5).ToObjectArray(),"cat".With("dog").ToArray());
        Subscribe(KeyCode.Z, () => {
            foreach(var s in lines) {
				Debug.Log("string: "+s);
			}
		});
	}
}
