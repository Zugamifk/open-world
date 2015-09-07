using UnityEngine;
using System.Collections;

public interface IWorldObject {

	Mesh mesh {
		get;
	}

	void InitializeWithWorldObject(WorldObject obj);

}
