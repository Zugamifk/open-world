using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Linq;

public static class Debugx {

	public struct Vector3 {
		public UnityEngine.Vector3 vector;
		public Color color;
		public Vector3(float x, float y, float z) {
            this.vector = new UnityEngine.Vector3(x, y, z);
			this.color = Color.white;
        }
		public Vector3(UnityEngine.Vector3 vector) : this(0,0,0){
			this.vector = vector;
		}
		public Vector3(float x, float y, float z, Color c) : this(x,y,z) {
			this.color = c;
        }
		public Vector3(UnityEngine.Vector3 vector, Color c) : this(vector){
			this.color = c;
		}
		public static implicit operator UnityEngine.Vector3(Vector3 v) {
            return v.vector;
        }
		public static explicit operator Vector3(UnityEngine.Vector3 v) {
            return new Vector3(v);
        }
		public static UnityEngine.Vector3 operator+(UnityEngine.Vector3 a, Vector3 b) {
			UnityEngine.Debug.DrawLine(a, b.vector, b.color);
            return a + b.vector;
        }
    }

	public static void DrawCross(UnityEngine.Vector3 position, float size, Color color, float duration = 0, bool depthTest = false) {
		UnityEngine.Debug.DrawLine(	position + UnityEngine.Vector3.up * size,
						position + UnityEngine.Vector3.down * size,
						color,
						duration,
						depthTest);
		UnityEngine.Debug.DrawLine(	position + UnityEngine.Vector3.left * size,
						position + UnityEngine.Vector3.right * size,
						color,
						duration,
						depthTest);
	}

    public static void DrawRect(Rect rect, Color color, float duration = 0, bool depthTest = false) {
		UnityEngine.Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), color, duration, depthTest);
		UnityEngine.Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), color, duration, depthTest);
		UnityEngine.Debug.DrawLine(new Vector3(rect.xMax, rect.yMax, 0), new Vector3(rect.xMax, rect.yMin, 0), color, duration, depthTest);
        UnityEngine.Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), color, duration, depthTest);
    }

    public static void Concat(string join, params object[] args)
    {
        UnityEngine.Debug.Log(string.Join(join, args.Select(o=>o.ToString()).ToArray()));
    }

    public static Stopwatch timer;
    public static void Tick() {
        timer = Stopwatch.StartNew();
    }

	public static void Tock() {
		if(timer==null) {
            UnityEngine.Debug.LogError("Timer not started!");
            return;
        } else {
            UnityEngine.Debug.Log("TIME: " + timer.Elapsed.Milliseconds+" ms");
        }
	}

    public static System.Action StartTimer()
    {
        var timer = Stopwatch.StartNew();
        return () =>
        {
            UnityEngine.Debug.Log("TIME: " + timer.Elapsed.Milliseconds+" ms");
            timer = Stopwatch.StartNew();
        };
    }

}
