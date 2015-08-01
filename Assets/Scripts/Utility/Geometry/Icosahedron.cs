using UnityEngine;
using System.Collections;
using Extensions;
//TODO GENERATE PROPER FACE NORMALS! 
namespace Geometry {
	public static partial class Shapes {
		public static Polyhedron Icosahedron() {
			var phi = Math.Phi;
			Vector3[] verts = new Vector3[12] {
				new Vector3(-1,phi,0),

				new Vector3(1,phi,0),
				new Vector3(0,1,phi),
				new Vector3(-phi,0,1),
				new Vector3(-phi,0,-1),
				new Vector3(0,1,-phi),

				new Vector3(phi,0,-1),
				new Vector3(phi,0,1),
				new Vector3(0,-1,phi),
				new Vector3(-1,-phi,0),
				new Vector3(0,-1,-phi),

				new Vector3(1,-phi,0)
			};

			int[][] edges = new int[30][];
			// edges around top point
			for(int i=0;i<5;i++) {
				edges[i] = new int[2] { 0, i+1};
			}

			// edge cicling top point
			for(int i=1;i<=5;i++) {
				edges[i+4] = new int[2] { i, i%5+1 };
			}

			// edges crossing halves
			for(int i=1;i<=5;i++) {
				edges[i+9] = new int[2] { i, i+5 };
				edges[i+14] = new int[2] { i, (i+1)%5 + 5 };
			}

			// edges circling bottom point
			for(int i=1;i<=5;i++) {
				edges[i+19] = new int[2] { i+5, (i+1)%5 + 5};
			}

			// edges around bottom points
			for(int i=1;i<=5;i++) {
				edges[i+24] = new int[2] { i + 5, 11};
			}

			int[][] faces = new int[20][];
			for(int i=0;i<5;i++) {
				faces[i] = new int[3] {i, i+5, (i+1)%5 };
			}

			for(int i=0;i<5;i++) {
				faces[i+5] = new int[3] { i+10, i+20, i+15};
				faces[i+10] = new int[3] { i+5, i+15, (i+1)%5 + 10};
			}

			for(int i=0;i<5;i++) {
				faces[i+15] = new int[3] {i+25, i+20, (i+1)%5 +25};
			}

			return new Polyhedron(verts, edges, faces);
 		}
	}
}
