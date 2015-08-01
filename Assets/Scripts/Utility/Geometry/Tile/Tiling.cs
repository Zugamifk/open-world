using UnityEngine;
using System.Collections;

namespace Geometry {
	public class Tiling : IMeshGenerator {

		public string name;

		private Tile[] tileset;
		private Graph<Tile> tiling;

		public string Name {
			get { return name; }
		}

		public Mesh Generate() {
			return new Mesh();
		}
	}
}
