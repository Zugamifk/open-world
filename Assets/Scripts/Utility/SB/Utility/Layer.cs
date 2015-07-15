using UnityEngine;
using System.Collections;
/* Utility class for handling gameobject layers. Layers can be defined
 	once here as then referenced easily in other scripts. The class
 	is partial so game-specific definitions can be made outside of
	HOEngine. */
public partial class Layer {
	
	/*Defined here are the three main camera layer groups (default, zoomup, and disabled zoomup)
		which each have an input-accepting and input-ignoring layer.
	*/
	public const int	Default = 0,
						IgnoreRaycast = 2,
						Zoomup = 8,
						ZoomupIgnoreRaycast = 9,
						ZoomupDisabled = 10,
						ZoomupDisabledIgnoreRaycast = 11;
						
	/* Sets a gameobject and all of its children to the specified layer. */
	public static void SetLayerRecursive(GameObject obj, int layer){
		obj.layer = layer;
		foreach(Transform child in obj.transform) {
			SetLayerRecursive(child.gameObject, layer);
		}
	}
	
	/* Gets the input-ignoring layer for the layer the given gameobject is on */
	public static int GetIgnore(GameObject o) {
		return GetIgnore(o.layer);
	}
	
	/* Gets the input-ignoring layer for the given layer */
	public static int GetIgnore(int originalLayer) {
		if (originalLayer <= IgnoreRaycast) {
			return IgnoreRaycast;
		} else if (originalLayer <= ZoomupIgnoreRaycast) {
			return ZoomupIgnoreRaycast;
		} else {
			return ZoomupDisabledIgnoreRaycast;
		}
	}
	
	/* Gets the input-accepting layer for the layer the given gameobject is on */
	public static int GetDefault(GameObject o) {
		return GetDefault(o.layer);
	}
	
	/* Gets the input-accepting layer for the given layer */
	public static int GetDefault(int originalLayer) {
		if (originalLayer <= IgnoreRaycast) {
			return Default;
		} else if (originalLayer <= ZoomupIgnoreRaycast){
			return Zoomup;
		} else {
			return ZoomupDisabled;
		}
	}
	
	/* Creates a bitmask out of the given layers */
	public static int Mask(params int[] layers) {
		int mask = 0;
		for (int i = 0; i < layers.Length; i++) {
			mask |= 1 << layers[i];
		}
		return mask;
	}
	
	/* Returns whether the gameobject's layer can receive input */
	public static bool IsInputLayer(GameObject obj){
		return IsInputLayer(obj.layer);
	}
	
	/* Returns whether the given layer can receive input */
	public static bool IsInputLayer(int layer){
		//If there's no zoomup, zoomups can't recieve input, and disabled
		//zoomups can never receive input
		return (layer == Default) || layer == Zoomup;
	}
	
	/* Returns whether the gameobject is on an active (i.e. not disabled) zoomup layer*/
	public static bool IsActiveZoomupLayer(GameObject obj){
		return IsActiveZoomupLayer(obj.layer);
	}
	
	/* Returns whether the given layer is an active (i.e. not disabled) zoomup layer */
	public static bool IsActiveZoomupLayer(int layer){
		return layer == Zoomup || layer == ZoomupIgnoreRaycast;
	}
	
	/* Returns whether the gameobject is on an "Active" layer, meaning it is in the
		active zoomup group or there is no zoomup and it is in the default group. */
	public static bool IsActiveLayer(GameObject obj){
		return IsActiveLayer(obj.layer);
	}
	
	/* Returns whether the layer is an "Active" layer, meaning it is in the
		active zoomup group or there is no zoomup and it is in the default group. */
	public static bool IsActiveLayer(int layer){
		return 	(layer == Default || layer == IgnoreRaycast);
	}
}
