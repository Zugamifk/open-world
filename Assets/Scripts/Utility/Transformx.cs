using UnityEngine;
using System.Collections;

public static class Transformx {

	public static void Zero(this Transform tf) {
		tf.localScale = Vector3.one;
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
    }

    public delegate void State(Transform tf);
	public static State Bookmark(this Transform tf) {
        var pos = tf.localPosition;
        var scl = tf.localScale;
        var rot = tf.localRotation;
        return ntf =>
        {
            ntf.localPosition = pos;
            ntf.localScale = scl;
            ntf.localRotation = rot;
        };
    }

    public static void OrientTo(this Transform child, Transform parent, bool keepOldParent = false) {
        Transform oldParent = parent;
		if(keepOldParent) oldParent = child.parent;
		child.parent = parent;
		child.Zero();
		if(keepOldParent) child.parent = oldParent;
    }
}
