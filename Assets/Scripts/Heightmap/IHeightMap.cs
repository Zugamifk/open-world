using UnityEngine;
using System.Collections;

public interface IHeightMap {

	float GetHeight(float x, float y);

	float GetHeight(Vector2 position);

	void GetHeight(ref Vector3 position);

}
