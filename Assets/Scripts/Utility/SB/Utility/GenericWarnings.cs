using UnityEngine;
using System.Collections;

public class GenericWarnings : MonoBehaviour {

	public static bool TestObject(Object go, GameObject parent) {
		if (go==null){
			Debug.LogWarning("Game object missing!", parent??go);
			return true;
		}
		return false;
	}

}
