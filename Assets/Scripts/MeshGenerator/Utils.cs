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
			// in the case we only send 2 verts, this is actually a number of successive vertices starting the first, a number equal tot he second
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
			for(int i = 0; i < fromIndices.Length; i++) {
	            to[startIndex + i] = from[fromIndices[i]];
	        }
		}

		/** Optimize mesh for unity */
		public static void PostGenerateMesh(Mesh mesh) {
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
            mesh.Optimize();
        }

	}
}
