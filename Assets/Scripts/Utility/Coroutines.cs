using UnityEngine;
using System.Collections;

public static class Coroutines {

	public static Coroutine WaitFor(this MonoBehaviour self, System.Func<bool> test) {
        return self.StartCoroutine(WaitFor(test));
    }

	public static IEnumerator WaitFor(System.Func<bool> test) {
		while(!test()) {
            yield return 1;
        }
	}
}
