using UnityEngine;
using System.Collections;
using Landscape;
using Extensions;

public class GroundScroller : MonoBehaviour {

	private static GroundScroller instance;
	void Awake() {
		this.SetInstanceOrKill(ref instance);
	}

	void Update() {
		var pos = Ground.TransformPoint(FPSControl.Position);
		var newPos = new Vector3(-Math.Mod(pos.x, 1), -pos.y, -Math.Mod(pos.z,1));
		Ground.TileRoot.localPosition = newPos;
		Ground.GridPosition = Vector3i.RoundDown(pos);
	}
}
