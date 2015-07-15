using UnityEngine;
using System.Collections;

public static class Gizmox {

	public static void Cross(Vector3 position, float length = 1) {
		Gizmos.DrawLine(position+Vector3.up*length,position+Vector3.down*length);
		Gizmos.DrawLine(position+Vector3.left*length,position+Vector3.right*length);
	}
}
