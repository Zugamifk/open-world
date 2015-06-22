using UnityEngine;
using System.Collections;

public class Interpolation {
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

	// _____________________________________________/ Curve generators \________
	public static Interpolation Power(float e) {
        return new Interpolation(
            t => Mathf.Pow(t, e)
		);
    }

	public static Interpolation Smooth(float i, float o) {
        return new Interpolation(
            t => Mathf.Pow(t, i)*(1-Mathf.Pow(1-t, o))
		);
    }

	public static Interpolation Smooth(float e) {
        return new Interpolation(
            t => Mathf.Pow(t, e)*(1-Mathf.Pow(1-t, e))
		);
    }

	// ___________________________________________/ Constructors \______________

	public Interpolation(Curve curve) {
		this.curve = curve;
	}

	public Interpolation(AnimationCurve curve) {
        this.curve = t => curve.Evaluate(t);
    }

	// ___________________________________________/ Operator Overloads \________

	public static Interpolation operator +(Interpolation a, Interpolation b) {
        return new Interpolation(t => a.curve(t) + b.curve(t));
    }

	public static Interpolation operator -(Interpolation a, Interpolation b) {
		return new Interpolation(t => a.curve(t) - b.curve(t));
	}

	public static Interpolation operator *(Interpolation a, Interpolation b) {
		return new Interpolation(t => a.curve(t) * b.curve(t));
	}

	public static Interpolation operator /(Interpolation a, Interpolation b) {
		return new Interpolation(t => a.curve(t) / b.curve(t));
	}

	public static implicit operator Interpolation(float f) {
        return new Interpolation(t => f);
    }

	public static implicit operator Interpolation(AnimationCurve c) {
		return new Interpolation(c);
	}
}
