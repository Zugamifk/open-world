using UnityEngine;
using System.Collections;

namespace Geometry {
	public class Tiling : IMeshGenerator {

		protected Tile[] tileset;
		protected Graph<Tile> tiling;

		public virtual string Name {
			get { return "Empty Tiling"; }
		}

		public virtual bool Match(Tile a, Tile b) {
			return true;
		}

		public virtual void Iterate() {

		}

		public virtual Mesh Generate() {
			return new Mesh();
		}
	}
}
