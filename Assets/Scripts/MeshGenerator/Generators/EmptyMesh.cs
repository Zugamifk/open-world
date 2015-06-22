using UnityEngine;
using System.Collections;

namespace MeshGenerator {
	public class EmptyMesh : IMeshGenerator {
		public float happy;

		public string Name {
			get {return "Empty Mesh";}
		}

		public Mesh Generate() {
			return new Mesh();
		}
	}
}
