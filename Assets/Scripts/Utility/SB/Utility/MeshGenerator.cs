using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshGenerator : MonoBehaviour {

	[SerializeField] MeshFilter meshfilter;
	[SerializeField] string meshName;

	private Mesh mesh;

	public delegate Vector3 VertexFunction(Vector3 point, int i);
	public delegate List<Vector3> VertexGenerator(int i);
	public delegate List<Vector3> VertexSorter(List<Vector3> points);
	public delegate List<Vector2> UVGenerator(List<Vector3> points);
	public delegate List<int> TriangleGenerator(List<Vector3> points);
	public struct MeshGeneratorPack {
		public int pointCount;
		public VertexGenerator vgen;
		public UVGenerator UVgen;
		public TriangleGenerator trigen;
	}

#if UNITY_EDITOR
	const string kAssetPath = "Assets/Models/";
	public void CreateAsset() {
		if(meshfilter == null) {
			Debug.LogWarning("No MeshFilter on object!");
			return;
		}
		mesh =  meshfilter.sharedMesh;
		if(mesh == null) {
			meshfilter.mesh = new Mesh();
			mesh= meshfilter.sharedMesh;
			AssetDatabase.CreateAsset(mesh, kAssetPath+meshName+".asset");
		}
		mesh.Clear();
	}

	public void Generate(VertexGenerator gen, int count, UVGenerator UVgen, TriangleGenerator triGen) {
		CreateAsset();
		var verts = gen(count);
		var UV = UVgen(verts);
		var tris = triGen(verts);

		mesh.vertices = verts.ToArray();
		mesh.uv = UV.ToArray();
		mesh.triangles = tris.ToArray();

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		EditorUtility.SetDirty(mesh);
		AssetDatabase.SaveAssets();
	}

	public void Generate(MeshGeneratorPack pack) {
		Generate(pack.vgen, pack.pointCount, pack.UVgen, pack.trigen);
	}
#endif

	// _________________________________________/ Public STATIC \_______________
	public static MeshGeneratorPack SpherePtGen(int rings, int sides) {
		MeshGeneratorPack pack;
		var da = 360/(float)sides;
		var dh = 180/((float)rings + 1);

		pack.pointCount = (2+rings)*(sides+1);

		// UV sphere on Y axis, with poles at y = +/- 1
		pack.vgen = i => {
			var pts = new List<Vector3>();
			var vec = Vector3.down;
			var a = 0f;
			var h = 0f;
			for(int r = 0; r < rings+2; r++) {
				a = 0;
				vec = Quaternion.AngleAxis(a, Vector3.up)*Quaternion.AngleAxis(h, Vector3.right)*Vector3.down;
				pts.Add(vec);
				for(int s = 0;s<sides; s++) {
					a += da;
					vec = Quaternion.AngleAxis(a, Vector3.up)*Quaternion.AngleAxis(h, Vector3.right)*Vector3.down;
					pts.Add(vec);
				}
				h += dh;
			}

			return pts;
		};

		// UV goes from bottom to top
		pack.UVgen = vl => {
			var UVs = new List<Vector2>();
			var a = 0f;
			var h = -0f;
			var v = -0f;
			for(int r = 0; r < rings+2; r++) {
				a = 0;
				for(int s = 0;s<sides; s++) {
					UVs.Add(new Vector2((a/360f)%1, v));
					a += da;
				}
				UVs.Add(new Vector2(1, v));
				h += dh;
				v = (h/180);
			}
			return UVs;
		};

		pack.trigen = vl => {
			var tris = new List<int>();
			for(int r = 0; r < rings+1; r++) {
				for(int s = 0;s<sides; s++) {
					tris.Add(r*(sides+1) + s);
					tris.Add(r*(sides+1) +(s+1));
					tris.Add((r+1)*(sides+1) + s);

					tris.Add(r*(sides+1) + (s+1));
					tris.Add((r+1)*(sides+1) + (s+1));
					tris.Add((r+1)*(sides+1) + s);
				}
			}
			tris.Reverse();
			return tris;
		};

		return pack;
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor {
	public override void OnInspectorGUI ()
	{
		var mg = target as MeshGenerator;
		DrawDefaultInspector();
		if(GUILayout.Button("Generate!")) {
			mg.Generate(MeshGenerator.SpherePtGen(6, 24));
		}
	}
}
#endif
