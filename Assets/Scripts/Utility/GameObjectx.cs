using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class GameObjectx {

	public static void SetLayerRecursively(this GameObject go, int layer) {
		go.layer = layer;
		foreach(Transform t in go.transform) {
			t.gameObject.SetLayerRecursively(layer);
		}
	}

	/** Set layer for all of a given object's children to match this parent */
	public static void SetLayerRecursively(this GameObject go) {
		foreach(Transform t in go.transform) {
			t.gameObject.SetLayerRecursively(go.layer);
		}
	}

	public static void AdjustRenderQueue(this GameObject go, int adjustment) {
		var rq = go.GetComponent<Renderer>().material.renderQueue;
			go.GetComponent<Renderer>().material.renderQueue = rq + adjustment;
	}

	public static void SetRenderersActive(this GameObject go, bool active) {
		foreach(var r in go.GetComponentsInChildren<Renderer>()) {
			r.enabled = active;
		}
	}

	// Returns minimum renderqueue of all renderers, before changing
	public static int RenderOver(this GameObject go, Renderer renderer) {
		// Get renderQueue for gameobject
		var q = renderer.material.renderQueue;
		// Get gameobject renderers
		var goRenderers = go.GetAll<Renderer>();
		if(goRenderers.Count()==0) return 0;
        // Get smallest renderqueue to set offsets when shifting renderqueues
        var queueBase = goRenderers.Select(r => r.material.renderQueue).Aggregate(
			(a,b) => Mathf.Min(a,b)
		);
		// Shift the renderQueues up
		goRenderers.ForEach(
			r => r.material.renderQueue =
				r.material.renderQueue - queueBase + q + 1);

		return queueBase;
	}

	// Renders all renderers relative to a new value
	public static void RenderRelativeTo(this GameObject go, int renderQueue) {
		// Get gameobject renderers
		var goRenderers = go.GetAll<Renderer>();
		// Get smallest renderqueue to set offsets when shifting renderqueues
		var queueBase = goRenderers.Select(r => r.material.renderQueue).Aggregate(
			(a,b) => Mathf.Min(a,b)
		);
		// Shift the renderQueues up
		goRenderers.ForEach(
			r => r.material.renderQueue =
				r.material.renderQueue - queueBase + renderQueue);
	}

	// Generic stuff
	public static void SetRenderersActive<T>(this GameObject go, bool active) where T : Renderer {
		foreach(var r in go.GetComponentsInChildren<T>()) {
			r.enabled = active;
		}
	}

    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T result = go.GetComponent<T>();
        if (result == null)
        {
            return go.AddComponent<T>();
        }
        else
        {
            return result;
        }
    }

	/** checks inactive gameobjects as well */
	public static IEnumerable<T> GetComponentsInAllChildren<T>(this GameObject go) where T:Component {
		var component = go.GetComponent(typeof(T));
		if (component!=null) yield return (T)component;
		foreach(Transform t in go.transform) {
			var components = t.gameObject.GetComponentsInAllChildren<T>();
			foreach(var c in components) {
				yield return c;
			}
		}
	}

	public static IEnumerable<T> GetAll<T>(this GameObject go) where T:Component {
		return go.GetComponentsInAllChildren<T>() as IEnumerable<T>;
	}

	public static void ForEach<T>(
		this GameObject go,
		Action<T> func
	) where T:Component {
		var items =  go.GetComponentsInChildren<T>();
		foreach(var i in items) {
			func(i);
		}
	}

	public static IEnumerable<T> Filter<T>(this IEnumerable<GameObject> gos) where T:Component {
		return gos.Select(g=>g.GetComponent<T>()).Where(c=>c!=null) as IEnumerable<T>;
	}
}
