using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Geometry;

namespace Landscape {
	public class Ground : MonoBehaviour {

		public Transform TileScalar;
		public Transform tileRoot;
		public TerrainGenerator Generator;
		public int Radius;
		public Transform origin;
		public int tileWidth, tileHeight;
		public Material groundMat;

		private IMeshGenerator groundTileMeshGenerator;

		private static Vector3i _GridPosition;
		public static Vector3i GridPosition {
			get { return _GridPosition; }
			set {
				var val = value;
				val.y=0;
				if( value != _GridPosition) {
					_GridPosition = value;
					instance.RefreshTiles();
				}
			}
		}
		public static Transform TileRoot {
			get { return instance.tileRoot; }
		}

		private Dictionary<Vector3i, Tile> worldTiles;
		private VisibleTile[,] tiles;

		public static Vector3 TransformPoint(Vector3 position) {
			return instance.TileScalar.InverseTransformPoint(position);
		}

		public static float GetHeight(Vector3 position) {
			var local = instance.TileScalar.InverseTransformPoint(position);
			return instance.TileScalar.localScale.y*instance.Generator.GetHeight(local.xz());
		}

		void SetTile(Vector3i position) {
			Tile tile = null;
			if (!worldTiles.TryGetValue(
				position,
				out tile)) {
				tile = new Tile();
				tile.position = position.xz();
				tile.map = Generator;
				tile.meshGenerator = groundTileMeshGenerator;
				tile.Mesh = tile.Generate();
				worldTiles.Add(position, tile);
			}
			var visible = tiles[
				position.x-GridPosition.x+Radius,
				position.z-GridPosition.z+Radius
			];
			visible.tile = tile;
			visible.Refresh();
		}

		void RefreshTiles() {
			var sqrRadius = Radius * Radius;
			for(int x = -Radius; x <= Radius; x++) {
				for(int y = -Radius; y <= Radius; y++) {
					if(x*x+y*y<=sqrRadius) {
						SetTile(new Vector3i(GridPosition.x+x, 0,GridPosition.z+y));
					}
				}
			}
		}

		public static Ground instance;
		void Awake() {
			this.SetInstanceOrKill(ref instance);
		}

		void Start() {
			var tileMeshGen = new Wang();
			tileMeshGen.width = tileWidth;
			tileMeshGen.height = tileHeight;
			tileMeshGen.numColors = 3;
			tileMeshGen.dropRate = 3;
			groundTileMeshGenerator = tileMeshGen;

			Generator.Init();
			GridPosition = (Vector3i)Vector3.zero;//origin.position;

			worldTiles = new  Dictionary<Vector3i, Tile>();
			tiles = new VisibleTile[2*Radius+1, 2*Radius+1];

			var sqrRadius = Radius * Radius;
			for(int x = -Radius; x <= Radius; x++) {
				for(int y = -Radius; y <= Radius; y++) {
					if(x*x+y*y<=sqrRadius) {
						var tileGO = new GameObject();
						tileGO.transform.OrientTo(tileRoot);
						tileGO.transform.localPosition = new Vector3(x,0,y);

						var tile = (VisibleTile)tileGO.AddComponent<VisibleTile>();
						tile.Init();
						tile.GetComponent<Renderer>().material = groundMat;

						tiles[x+Radius, y+Radius] = tile;
						SetTile(new Vector3i(GridPosition.x+x,0,GridPosition.z+y));
					}
				}
			}
		}

		void Update() {
			gameObject.name = "ground: "+GridPosition;
		}

	}
}
