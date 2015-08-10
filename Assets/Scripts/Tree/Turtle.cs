using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Extensions;
using MeshGenerator;

public class Turtle : MonoBehaviour {

	public LSystem system = new LSystem();
	public readonly string[] alphabet = {
		"F", "+", "-"
	};

	public string current = "";
	public int defaultIterations = 0;
	public float angleStep;
	public float step;
	public float[] unused;

	public static Turtle instance;
	void Awake() {
		this.SetInstanceOrKill(ref instance);
	}

	public Polygon Path(int derivations) {
		current = system.ElementAt(derivations);
		var heading = Vector2.up;
		var angle = 0f;
		var pos = Vector2.zero;
		var points = new List<Vector2>();
		bool turned = true;
		for(int i=0;i<current.Count();i++) {
			switch(current[i]) {
				case 'F': {
					if(turned) {
						points.Add(pos);
						turned = false;
					}
					pos += heading.Rotate(angle)*step;
				} break;
				case '+': {
					angle += angleStep;
					turned = true;
				} break;
				case '-': {
					angle -= angleStep;
					turned = true;
				} break;
				default: Debug.Log("Bad character in string: \'"+current[i]+"\'"); break;
			}
		}
		points.Add(pos);
		return new Polygon(points.ToArray());
	}

	public class TurtleMesh : IMeshGenerator {
		public Turtle turtle;
		public int depth;
		public Color colorStart;
		public Color colorEnd;
		public string Name {
			get { return "Turtle Path Mesh";}
		}
		public Mesh Generate() {
			var path = turtle.Path(depth);
			var mesh = path.GenerateMesh();
			mesh.SplitTriangles();
			mesh.ColorByVertexIndex(colorStart, colorEnd);
			turtle.unused = new float[path.Vertices.Count];
			MeshGenerator.Utils.PostGenerateMesh(mesh);
			return mesh;
		}
	}
	private const int maxGizmos = 512;
	public void OnDrawGizmos() {
		var path = Path(defaultIterations);
		var startCol = (ColorHSV)Colorx.FromHex(0xFF00FF);
		var endCol = (ColorHSV)Colorx.FromHex(0x00FF00);
		var len = Mathf.Min(path.Vertices.Count, maxGizmos);
		for(int i=0;i<len;i++) {
			var col = ColorHSV.Lerp(startCol, endCol, (float)i/(float)len);
			Gizmos.color = col;
			Gizmos.DrawSphere(transform.position+(Vector3)path.Vertices[i].value, 0.2f);
			Gizmos.DrawLine(
				transform.position+(Vector3)path.Vertices[i].value,
				transform.position+(Vector3)path.Vertices[(i+1)%path.Vertices.Count].value
			);
		}
	}
}
