using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Landscape {
	public class Ground : MonoBehaviour {

		public Transform TileScalar;
		public Transform tileRoot;
		public TerrainGenerator Generator;
		public int Radius;
		public Transform origin;
		public int tileWidth, tileHeight;
		public Material groundMat;

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

		private Dictionary<Vector3i, Mesh> meshes;
		private Tile[,] tiles;

		public static Vector3 TransformPoint(Vector3 position) {
			return instance.TileScalar.InverseTransformPoint(position);
		}

		public static float GetHeight(Vector3 position) {
			var local = instance.TileScalar.InverseTransformPoint(position);
			return instance.TileScalar.localScale.y*instance.Generator.GetHeight(local.xz());
		}

		void EnableMesh(Vector3i position) {
			Mesh mesh = null;
			var tile = tiles[position.x-GridPosition.x+Radius, position.z-GridPosition.z+Radius];
			tile.position = position.xz();
			tile.gameObject.name = tile.Name;
			if (!meshes.TryGetValue(position, out mesh)) {
				mesh = tile.Generate();
				meshes.Add(position, mesh);
			}
			tile.Mesh = mesh;
		}

		void RefreshTiles() {
			var sqrRadius = Radius * Radius;
			for(int x = -Radius; x <= Radius; x++) {
				for(int y = -Radius; y <= Radius; y++) {
					if(x*x+y*y<=sqrRadius) {
						EnableMesh(new Vector3i(GridPosition.x+x, 0,GridPosition.z+y));
					}
				}
			}
		}

		public static Ground instance;
		void Awake() {
			this.SetInstanceOrKill(ref instance);
		}

		void Start() {
			Generator.Init();
			GridPosition = (Vector3i)Vector3.zero;//origin.position;
			meshes = new  Dictionary<Vector3i, Mesh>();
			tiles = new Tile[2*Radius+1, 2*Radius+1];
			var sqrRadius = Radius * Radius;
			for(int x = -Radius; x <= Radius; x++) {
				for(int y = -Radius; y <= Radius; y++) {
					if(x*x+y*y<=sqrRadius) {
						var tileGO = new GameObject();
						var tile = (Tile)tileGO.AddComponent<Tile>();
						tile.position = new Vector2(GridPosition.x+x,GridPosition.z+y);
						tile.map = Generator;
						tile.gridWidth = tileWidth;
						tile.gridHeight = tileHeight;
						tileGO.name = tile.Name;

						var renderer = tileGO.AddComponent<MeshRenderer>();
						renderer.material = groundMat;

						tileGO.AddComponent<MeshFilter>();

						meshes.Add((Vector3i)tile.position.x0y(), tile.Generate());

						tileGO.transform.OrientTo(tileRoot);
						tileGO.transform.localPosition = new Vector3(x,0,y);

						tiles[x+Radius, y+Radius] = tile;
					}
				}
			}
		}

		void Update() {
			gameObject.name = "ground: "+GridPosition;
		}

	}
}
