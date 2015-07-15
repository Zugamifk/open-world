/*____________________________________________________________________________

AnimationExt

____________________________________________________________________________*/

using UnityEngine;
using System.Collections;

public static class AnimationExt {

	/*

	//  --- Nicholas Francis' old system ---

	public static IEnumerator YieldForPlay(int hash, string layerName, System.Action Callback, float crossFade, float repeatCount){

		Animator anim = GetComponent<Animator>();

		int layer = 0;

		for (int i = 0; i < anim.layerCount; i++){
			if ( anim.GetLayerName(i) == layerName ){
				layer = i;
			}
		}

		anim.CrossFade (hash, crossFade, layer, 0f);

		while (anim.GetCurrentAnimatorStateInfo (layer).nameHash != hash){
			yield return 0;
		}

		float currentTime = Mathf.Floor(anim.GetCurrentAnimatorStateInfo (layer).normalizedTime);

		while (anim.GetCurrentAnimatorStateInfo(layer).nameHash == hash && anim.GetCurrentAnimatorStateInfo(layer).normalizedTime < currentTime + repeatCount){
			yield return 0;
		}

		if (Callback != null){
			Callback();
		}

	}
	*/

	public static IEnumerator SetTriggerAndYield(this Animator animator, string triggerString){

		var currentStateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

		animator.SetTrigger(triggerString);

		while (animator.GetCurrentAnimatorStateInfo(0).nameHash == currentStateHash){
			yield return 0;
		}

		var newStateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

		float currentTime = Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

		while (
			animator.GetCurrentAnimatorStateInfo(0).nameHash == newStateHash &&
			animator.GetCurrentAnimatorStateInfo(0).normalizedTime < currentTime + 1
		){
			yield return 0;
		}

		yield break;

	}

	public static IEnumerator WaitForAnimation(this Animator animator){

		var newStateHash = animator.GetCurrentAnimatorStateInfo(0).nameHash;

		float currentTime = Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

		while (
			animator.GetCurrentAnimatorStateInfo(0).nameHash == newStateHash &&
			animator.GetCurrentAnimatorStateInfo(0).normalizedTime < currentTime + 1
		){
			yield return 0;
		}

		yield break;

	}

	public static IEnumerator AnimateAndYield(this Animation animation){

		while(animation.isPlaying) {
			yield return 1;
		}

	}
}
