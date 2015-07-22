using UnityEngine;
using System.Collections;
using MeshGenerator;

namespace Landscape {
	public class Tile : MonoBehaviour, IMeshGenerator {

		public Vector2 position;
		public IHeightMap map;
		public int gridWidth, gridHeight;
		public bool smooth = true;
		public bool tile;

		private Mesh mesh;
		private MeshFilter filter;
		public Mesh Mesh {
			get { return mesh; }
			set {
				mesh = value;
				if(filter != null) filter.mesh = value;
			}
		}

		const string _tileNamePrefix = "tile ";
		public string Name {
			get {
				return _tileNamePrefix + position;
			}
		}

		public Mesh Generate() {
			// Extreme points of plane
            var pts = new Vector3[] {
				Vector3.zero,
				Vector3.forward,
				new Vector3(1,0,1),
				Vector3.right
			};

            // width of a tile in the plane
            var xs = 1f / (float)gridWidth;
            var ys = 1f / (float)gridHeight;

			var verts = new Vector3[(gridWidth + 1) * (gridHeight + 1)];

            // Fill verts
			for(int y=0;y<=gridHeight;y++) {
				for(int x=0;x<=gridWidth;x++) {
					verts[x + (gridWidth+1)*y]
						= new Vector3(
							(float)x*xs,
							map.GetHeight(position.x+(float)x*xs, position.y+(float)y*ys),
							(float)y*ys
							);
				}
			}

            var tris = new int[gridHeight * gridWidth * 6];
            var uv = new Vector2[verts.Length];

            if (smooth)
            {
                // fill grid tiles
                var ti = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        var xi0 = y * (gridWidth + 1) + x;
                        var xi1 = (y + 1) * (gridWidth + 1) + x;
                        Utils.AddFace(tris, ti * 6, xi0 + 1, xi0, xi1, xi1 + 1);
                        ti++;
                    }
                }

                uv.Fill(verts, v => new Vector2(v.x, v.z));
            }
            else
            {
                var grid = new Vector3[(gridWidth + 1) * (gridHeight + 1)];
                verts.CopyTo(grid, 0);

                verts = new Vector3[gridWidth * gridHeight * 4];
                uv = new Vector2[verts.Length];

                // fill grid tiles
                var ti = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        var xi0 = y * (gridWidth + 1) + x;
                        var xi1 = (y + 1) * (gridWidth + 1) + x;
                        Utils.DuplicateVerts(grid, verts, ti * 4, xi0 + 1, xi0, xi1, xi1 + 1);
                        Utils.AddFace(tris, ti * 6, ti * 4, 4);
                        ti++;
                    }
                }

                if (tile)
                {
                    var tileUV = new Vector2[]
                    {
                    Vector2.right,
                    Vector2.zero,
                    Vector2.up,
                    new Vector2(1,1)
                    };
                    uv.Fill(tileUV, c => c);
                }
                else
                {
                    uv.Fill(verts, v => new Vector2(v.x, v.z));
                }

            }

            // fill new mesh object
            mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;
			mesh.name = Name;

			Utils.PostGenerateMesh(mesh);

			var filter = GetComponent<MeshFilter>();
			if(filter!=null) filter.mesh = mesh;

			return mesh;
		}

		void Start() {
			filter = GetComponent<MeshFilter>();
		}
	}
}
