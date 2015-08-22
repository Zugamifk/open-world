using UnityEngine;
using System.Collections;
using UnityEditor;

public class TurtleDrawer3D : MonoBehaviour {

    public Turtle3D turtle;
    public bool drawNodes;
    public float nodeRadius = 0.2f;
    [HideInInspector]
    public bool autoGenerate;

    private DirectedGraph<Vector3> path;

    public void GeneratePath() {
        if(turtle == null) return;
        path = turtle.Path(turtle.defaultIterations);
    }

    private const int maxGizmos = 2048;
	public void OnDrawGizmos()
	{
		if (path == null) return;

		var startCol = (ColorHSV)Colorx.FromHex(0xFF00FF);
		var endCol = (ColorHSV)Colorx.FromHex(0x00FF00);
		var len = Mathf.Min(path.Vertices.Count, maxGizmos);
		for (int i = 0; i < len; i++)
		{
			var col = ColorHSV.Lerp(startCol, endCol, (float)i / (float)len);
			Gizmos.color = col;
			if(drawNodes)
                Gizmos.DrawSphere(transform.position + (Vector3)path.Vertices[i].value, nodeRadius);
			foreach (var e in path.Vertices[i].Outgoing)
			{
				Gizmos.DrawLine(
					transform.position + (Vector3)e.from.value,
					transform.position + (Vector3)e.to.value
				);
			}
		}
	}
}
