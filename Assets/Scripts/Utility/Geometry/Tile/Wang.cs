using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Geometry;

namespace Geometry {
	public class Wang : Tiling {
		public class Domino : Tile{
			public int topID;
			public int bottomID;
			public int leftID;
			public int rightID;
			public Domino(int t, int b, int l, int r) {
				topID = t;
				bottomID = b;
				leftID = l;
				rightID = r;
			}
		}

		public int width;
		public int height;
		public int dropRate;
		public int numColors;
		private int[][] tiles;
		private List<Color> colors;

		public Wang() {
			tiles = new int[width][];
			for(int i=0;i<width;i++) {
				tiles[i] = new int[height];
			}
			colors = new List<Color>();
		}

		public Wang(int[][] sideIDs, Color[] colors) : this() {
			SetTiles(sideIDs);
			SetColors(colors);
		}

		public void Resize() {
			tiles = new int[width][];
			for(int i=0;i<width;i++) {
				tiles[i] = new int[height];
			}
		}

		public void SetTiles(int[][] sideIDs) {
			tileset = new Domino[sideIDs.Length];
			for(int i=0;i<tileset.Length;i++) {
				tileset[i] = new Domino(sideIDs[i][0],sideIDs[i][1],sideIDs[i][2],sideIDs[i][3]);
			}
			// Debug.Log("Set has "+tileset.Length+" Wang tiles!");
		}

		public void SetColors(params Color[] colors) {
			this.colors = colors.ToList();
			numColors = colors.Length;
		}

		public override string Name {
			get { return "Wang Tiling"; }
		}

		public void Fill() {
			Domino[] dominos = tileset.Select(t=>(Domino)t).ToArray();
			IEnumerable<int> allLegal = Enumerable.Range(0,dominos.Length);
			IEnumerable<int> legal = allLegal;
			for(int x=0;x<width;x++) {
				for(int y = 0;y<height;y++) {
					legal = allLegal;
					if(x>0) {
						legal = legal.Where(d=>dominos[d].leftID == dominos[tiles[x-1][y]].rightID);
					}
					if(y>0) {
						legal = legal.Where(d=>dominos[d].bottomID == dominos[tiles[x][y-1]].topID);
					}
					if(legal.Count()>0) {
						tiles[x][y] = legal.Random();
					} else {
						// Debug.LogError("Wang Tile Fill error! Couldn't Fill space width current tiles!");
						return;
					}
				}
			}
		}

		public Mesh GenerateTileMesh(Domino d) {
			var pts = new Vector3[] {
				new Vector3(0.5f,0.5f,0),
				Vector3.zero, Vector3.right, new Vector3(1,1,0), Vector3.up
			};
			var verts = new List<Vector3>();
			var colorIDs = new int[]{d.bottomID,d.rightID,d.topID,d.leftID};
			var vColors = new List<Color>();

			for(int i=0;i<4;i++) {
				verts.Add(pts[0]);
				verts.Add(pts[(i+1)%4+1]);
				verts.Add(pts[i+1]);
				if(colors.Count <= colorIDs[i]) {
					Debug.LogWarning("Not enough colors specified in this tileset!");
					colorIDs[i] = 0;
				}
				vColors.Add(colors[colorIDs[i]]);
				vColors.Add(colors[colorIDs[i]]);
				vColors.Add(colors[colorIDs[i]]);
			}

			var tris = Enumerable.Range(0,12).ToArray();
			var uvs = verts.Select(v=>v.xy()).ToArray();

			var mesh = new Mesh();
			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.colors = vColors.ToArray();
			return mesh;
		}

		public override Mesh Generate() {
			var num = Mathf.Max(1,numColors);
			var pick = Enumerable.Range(0,num);
			SetTiles(
				pick.Choose(4,true).Shuffle().Where((t,i)=>i%dropRate==0).Select(seq=>seq.ToArray()).ToArray()
			);
			SetColors(Colorx.FibonacciHues(new ColorHSV(Random.value, 1,1)).Take(num).ToArray());
			Resize();
			Fill();
			var tileMeshes = tileset.Select(t=>GenerateTileMesh((Domino)t)).ToList();

			var verts = new List<Vector3>();
			var tris = new List<int>();
			var uvs = new List<Vector2>();
			var vColors = new List<Color>();
			Vector3 offset = Vector3.zero;
			int triangleIndexBase = 0;
			for(int x = 0;x<width;x++) {
				for(int y = 0;y<height;y++) {
					var tile = tileMeshes[tiles[x][y]];
					verts.AddRange(tile.vertices.Select(v=>v+offset));
					tris.AddRange(tile.triangles.Select(i=>i+triangleIndexBase));
					uvs.AddRange(tile.uv);
					vColors.AddRange(tile.colors);
					offset.y+=1;
					triangleIndexBase += tile.vertexCount;
				}
				offset.y=0;
				offset.x+=1;
			}
			var mesh = new Mesh();
			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.colors = vColors.ToArray();
			return mesh;
		}
	}
}
