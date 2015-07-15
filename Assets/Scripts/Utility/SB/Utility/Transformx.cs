using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void TransformBookmark(Transform t);

public static class Transformx {

	public static IEnumerable<Transform> GetChildren(this Transform tf) {
		foreach(Transform t in tf) {
			yield return t;
		}
	}

	public static void ToChildren(this Transform tf, Action<Transform> func) {
        foreach (Transform t in tf)
        {
            func(t);
        }
    }

	/** Stores a transform's current state in a closure and returns a function
		that takes a transform and applies that state to it */
    public static TransformBookmark Remember(this Transform tf) {
		var pos = tf.localPosition;
        var scl = tf.localScale;
        var rot = tf.localRotation;
        return (Transform t) =>
        {
            t.localPosition = pos;
            t.localScale = scl;
            t.localRotation = rot;
        };
    }

	/** Stores a transform's current state in a closure and returns a function
		that takes a transform and applies that state to it. This version also
		acts recursively on each of the given transform's children, and expects
		a given transform's hierarchy to match the initial one. */
	public static TransformBookmark RememberRecursive(this Transform tf) {
		var pos = tf.localPosition;
        var scl = tf.localScale;
        var rot = tf.localRotation;
        var childBookmarks = tf.GetChildren().Select(
            child => child.RememberRecursive()
        ).ToList();
        return (Transform t) =>
        {
            t.localPosition = pos;
            t.localScale = scl;
            t.localRotation = rot;
            t.GetChildren().ForEach(
                childBookmarks,
                (child, bookmark) => bookmark(child)
            );
        };
    }

	/** Sets a transform's local position/rotation to 0 and local scale to 1*/
	public static void Zero(this Transform tf) {
		tf.localPosition = Vector3.zero;
		tf.localRotation = Quaternion.identity;
		tf.localScale = Vector3.one;
	}

	/** parents and zeros a transform */
	public static void CentreOn(this Transform tf, Transform parent) {
		tf.transform.parent = parent;
		tf.Zero();
	}

}
