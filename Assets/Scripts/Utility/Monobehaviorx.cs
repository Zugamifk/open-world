using UnityEngine;
using System.Collections;

public static class Monobehaviorx {

	/** Set a singleton */
	public static void SetInstanceOrKill<T>(this T self, ref T instance, string debug = "")
	where T : MonoBehaviour {
		if (instance !=null) {
			if (debug!="") Debug.LogWarning(debug, instance);
            GameObject.Destroy(self.gameObject);
			return;
        }

		instance = self;
	}

}
