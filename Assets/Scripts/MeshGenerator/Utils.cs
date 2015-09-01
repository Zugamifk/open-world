using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace MeshGenerator {
	public static class Utils {

		public static IEnumerable<int> GetFacePoints(params int[] verts) {
			for (int i=0; i<verts.Length-2; i++) {
	            yield return verts[0];
				yield return verts[i+1];
				yield return verts[i+2];
	        }
		}

		public static int AddFace(int[] triangleArray, int startIndex, params int[] verts) {
	        int num = 0;
			// in the case we only send 2 verts, this is actually a number of successive vertices starting from the first, a number equal to the second
			if (verts.Length == 2) {
	            verts = Enumerable.Range(verts[0], verts[1]).ToArray();
	        }

	    	foreach(var pt in GetFacePoints(verts)) {
	            triangleArray[startIndex + num] = pt;
	            num++;
	        }
	        return num;
	    }


		public static IEnumerable<Vector3> GetRingPoints(int numPoints, Vector3 axis, Vector3 initialPoint) {
	        float angle = 0;
			float step = 360/(float)numPoints;
	        for(int i=0;i<numPoints;i++) {
	            yield return Quaternion.AngleAxis(angle, axis) * initialPoint;
				angle+=step;
	        }
		}

		public static void DuplicateVerts(Vector3[] from, Vector3[] to, int startIndex, params int[] fromIndices) {
			if(fromIndices.Length==2) {
				if(fromIndices[1]<0) {
					for (int i = 0; i < -fromIndices[1]; i++)
                    {
                        to[startIndex + i] = from[fromIndices[0]-(fromIndices[1] + i + 1)];
                    }
				} else
                {
                    for (int i = 0; i < fromIndices[1]; i++)
                    {
                        to[startIndex + i] = from[fromIndices[0] + i];
                    }
                }
            } else
            {
                for (int i = 0; i < fromIndices.Length; i++)
                {
                    to[startIndex + i] = from[fromIndices[i]];
                }
            }
        }

		/** flip triangles facing in a triangleArray array */
		public static void FlipTriangles(int[] verts) {
			for(int t = 0;t<verts.Length;t+=3) {
				var temp = verts[t];
				verts[t] = verts[t+1];
				verts[t+1] = temp;
			}
		}

		/** Optimize mesh for unity */
		public static void PostGenerateMesh(Mesh mesh) {
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
            mesh.Optimize();
        }

		// EXTENSION METHODS
		public static void ColorByVertexIndex(this Mesh mesh, Color startColor, Color endColor) {
			var colors = new Color[mesh.vertexCount];
			for(int i=0;i<colors.Length;i++) {
				colors[i] = Color.Lerp(startColor, endColor, (float)i/(float)colors.Length);
			}
			mesh.colors = colors;
		}
		public static void ColorByLastTriangleIndex(this Mesh mesh, Color startColor, Color endColor) {
			var colors = new Color[mesh.vertexCount];
			var tris = mesh.triangles;
			for(int i=0;i<tris.Length;i++) {
				colors[tris[i]] = Color.Lerp(startColor, endColor, (float)i/(float)tris.Length);
			}
			mesh.colors = colors;
		}

		public static void SplitTriangles(this Mesh mesh) {
			var verts = mesh.vertices;
			var tris = mesh.triangles;
			if(verts.Length == tris.Length) return;

			var newVerts = new Vector3[tris.Length];
			var used = new bool[verts.Length];
			for(int i=0;i<newVerts.Length;i++) {
				newVerts[i] = verts[tris[i]];
				tris[i] = i;
			}
			mesh.vertices = newVerts;
			mesh.triangles = tris;
		}

	}
}
