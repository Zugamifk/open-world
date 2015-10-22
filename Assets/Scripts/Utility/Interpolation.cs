using UnityEngine;
using System.Collections;

public class AnimatedCurve {
	// _________________________________________________/ Fields \______________

    public delegate float Curve(float t);
    public Curve curve;

    // _________________________________________________/ Methods \_____________

    /** animate a change in a value */
    public IEnumerator AnimateValue(float from, float to, float time, System.Action<float> setCallback) {
        setCallback(from);
        for(float t=0;t<1;t+=Time.deltaTime/time) {
			setCallback(Mathf.Lerp(from, to, curve(t)));
            yield return 1;
        }
        setCallback(to);
        yield break;
	}

	public IEnumerator AnimateValue(float time, System.Action<float> setCallback) {
        setCallback(curve(0));
        for(float t=0;t<1;t+=Time.deltaTime/time) {
			setCallback(curve(t));
            yield return 1;
        }
        setCallback(curve(1));
        yield break;
	}

    private Material mat;
    private const float segments = 100;
    public void Draw() {
        if (!mat)
        {
            var shader = Shader.Find ("Hidden/Internal-Colored");
            mat = new Material (shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt ("_ZWrite", 0);
            mat.SetInt ("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }
        mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadOrtho ();
        GL.Begin(GL.LINES);
        float t = 0;
        GL.Vertex3(0, curve(0), 0);
        for(float i=0;i<segments;i++) {
            t = i / segments;
            GL.Vertex3(t, curve(t), 0);
            GL.Vertex3(t, curve(t), 0);
        }
        GL.End();
        GL.PopMatrix();
    }

	// _____________________________________________/ Curve generators \________
	public static AnimatedCurve Power(float e) {
        return new AnimatedCurve(
            t => Mathf.Pow(t, e)
		);
    }

	public static AnimatedCurve Smooth(float e) {
        var cr = Mathf.Exp(e*0.5f);
        return new AnimatedCurve(
            t => Mathf.Exp(e * t) / (cr + Mathf.Exp(e * t))
		);
    }

    public static AnimatedCurve Bump(float i, float o) {
        return new AnimatedCurve(
            t => Mathf.Pow(t,i) * Mathf.Pow(1 - t, o)
        );
    }

	// ___________________________________________/ Constructors \______________

	public AnimatedCurve(Curve curve) {
		this.curve = curve;
	}

	public AnimatedCurve(AnimationCurve curve) {
        this.curve = t => curve.Evaluate(t);
    }

	// ___________________________________________/ Operator Overloads \________

	public static AnimatedCurve operator +(AnimatedCurve a, AnimatedCurve b) {
        return new AnimatedCurve(t => a.curve(t) + b.curve(t));
    }

	public static AnimatedCurve operator -(AnimatedCurve a, AnimatedCurve b) {
		return new AnimatedCurve(t => a.curve(t) - b.curve(t));
	}

	public static AnimatedCurve operator *(AnimatedCurve a, AnimatedCurve b) {
		return new AnimatedCurve(t => a.curve(t) * b.curve(t));
	}

	public static AnimatedCurve operator /(AnimatedCurve a, AnimatedCurve b) {
		return new AnimatedCurve(t => a.curve(t) / b.curve(t));
	}

	public static implicit operator AnimatedCurve(float f) {
        return new AnimatedCurve(t => f);
    }

	public static implicit operator AnimatedCurve(AnimationCurve c) {
		return new AnimatedCurve(c);
	}

    public static implicit operator AnimatedCurve(Curve c) {
		return new AnimatedCurve(c);
	}
}
