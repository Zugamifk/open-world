using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spline {
	public delegate Vector3 PointFunction();
	private struct PointUpdater {
		public PointFunction func;
		public int index;
		public PointUpdater(PointFunction f, int i) {
			func = f;
			index = i;
		}
	}
	[SerializeField] public List<Vector3> Points;
	private List<PointUpdater> m_pointFunctions;
	private List<System.Action> m_updateFunctions;

	// this is wrong when chords aren't unit length
	public float ChordLength {
		get { return (float)Points.Count-1; }
	}

	public int iEnd {
		get { return Points.Count-1; }
	}

	private void Init() {

	}

	public Spline() {
		Points = new List<Vector3>();
		m_pointFunctions = new List<PointUpdater>();
		m_updateFunctions = new List<System.Action>();
	}

	public Spline(List<Vector3> points) : this() {
		foreach(var p in points) {
			this.Points.Add (p);
		}
	}

	public void Add(Vector3 point) {
		Points.Add(point);
	}

	public void Add(Transform tf, bool track = false) {
		Points.Add(tf.position);
		if(track) AddTransformPointUpdater(tf, Points.Count-1);
	}

	public void AddUpdaterFunction(System.Action f) {
		m_updateFunctions.Add (f);
	}

	public void AddPointUpdater(PointFunction f,int i) {
		m_pointFunctions.Add(new PointUpdater(f, i));
	}

	public void AddTransformPointUpdater(Transform t, int i) {
		AddPointUpdater(()=>t.position, Mathf.Clamp(i, 0, Points.Count-1));
	}

	public void AddInterpolatingPointUpdater(Mathfx.ParametricCurve falloff, int index, int influence) {
		Vector3 op = Points[index];
		Vector3 step = Vector3.zero;
		AddUpdaterFunction(()=>{
			step = Points[index]-op;
			op = Points[index];
		});
		for(int i = Mathf.Max(0, index-influence); i < Mathf.Min (Points.Count, i+influence); i++) {
			if(i==index) continue;
			int it = i;
			float t = 1-(float)Mathf.Abs(i-index)/(float)influence;
			AddPointUpdater( () => Points[it] + falloff(t)*step, i);
		}
	}

	public void Update() {
		foreach(var u in m_updateFunctions) {
			u();
		}
		foreach(var pu in m_pointFunctions) {
			Points[pu.index] = pu.func();
		}
	}

	public Vector3 Evaluate(float t) {
		if (Points.Count<2) {
			Debug.LogWarning("Spline needs more points!");
			return Vector3.zero;
		}
		t = Mathf.Clamp(t, 0, Points.Count-1.000001f);
		int i = Mathf.Clamp(Mathf.FloorToInt(t), 0, Points.Count-2);
		var m0 = (Points[i+1]-Points[i==0?i:i-1])/2;
		var m1 = (Points[i+2==Points.Count?i+1:i+2]-Points[i])/2;

		t = t%1;
		var h00 = (1+2*t)*(1-t)*(1-t);
		var h10 = t*(1-t)*(1-t);
		var h01 = t*t*(3-2*t);
		var h11 = t*t*(t-1);

		return h00*Points[i] + h10*m0 + h01*Points[i+1] + h11*m1;
	}

	/** Doesn't work right now, could be fiquesed*/
	public Vector3 EvaluateDerivative(float t) {
		if (Points.Count<2) {
			Debug.LogWarning("Spline needs more points!");
			return Vector3.zero;
		}
		t = Mathf.Clamp(t, 0, Points.Count-1.000001f);
		int i = Mathf.Clamp(Mathf.FloorToInt(t), 0, Points.Count-2);
		var m0 = (Points[i+1]-Points[i==0?i:i-1])/2;
		var m1 = (Points[i+2==Points.Count?i+1:i+2]-Points[i])/2;

		t = t%1;
		var h00 = 6*(t*t-t);
		var h10 = 3*t*t-4*t+1;
		var h01 = -6*(t*t-t);
		var h11 = 3*t*t-2*t;

		return h00*Points[i] + h10*m0 + h01*Points[i+1] + h11*m1;
	}

	public Vector3 EvaluateNormalized(float t) {
		return Evaluate(t*(Points.Count-1));
	}

	public Vector3 EvaluateDerivativeNormalized(float t) {
		return EvaluateDerivative(t*(Points.Count-1));
	}

	public void DebugDraw(float step = 0.05f, bool drawControlPoints = false) {
		if (Points.Count<2) return;
		Vector3 from = Points[0];
		for(float t = 0;t<ChordLength;t+=step) {
			var to = Evaluate (t);
			var ci = t/(float)Points.Count;
			Debug.DrawLine(from, to, new Color(ci*ci, ci*(1-ci)*4, (1-ci)*(1-ci)));
			from = to;
		}
		if(drawControlPoints) {
			for(int i=0;i<Points.Count;i++) {
				DebugDrawPos((float)i);
			}
		}
	}

	public void DebugDraw(Transform parent, float step = 0.05f) {
		if (Points.Count<2) return;
		Vector3 from = Points[0];
		for(float t = 0;t<ChordLength;t+=step) {
			var to = Evaluate (t);
			var ci = t/(float)Points.Count;
			Debug.DrawLine(parent.TransformPoint(from), parent.TransformPoint(to), new Color(ci*ci, ci*(1-ci)*4, (1-ci)*(1-ci)));
			from = to;
		}
	}

	public void DebugDrawPos(float t) {
		var fwd = Evaluate(t)-Evaluate(t+0.05f);
		var pos = Evaluate(t);
		var perp1 = Vector3.Cross (fwd, Vector3.up);
		var perp2 = Vector3.Cross(perp1, fwd);
		var ci = t/(float)Points.Count;
		var col = new Color(ci*ci, ci*(1-ci)*4, (1-ci)*(1-ci));
		Debug.DrawLine(pos+perp1, pos-perp1, col);
		Debug.DrawLine(pos+perp2, pos-perp2, col);
	}
}
