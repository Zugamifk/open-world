using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraSingleton : MonoBehaviour {

    public Lazy<Camera> cameraRef;
    public static Camera Camera {
        get { return instance.cameraRef.Value; }
    }

    public static CameraSingleton instance;
    void Awake () {
		this.SetInstanceOrKill(ref instance, "CameraSingelton already set! Destroying duplicate instance.");
		cameraRef = new Lazy<Camera>(()=>(Camera)gameObject.GetComponent(typeof(Camera)));
	}

}
