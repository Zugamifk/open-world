/*____________________________________________________________________________

AnimatedValue

Encapulates an animation with an updating value, interpolating from old values to
new values using the enclosed function.
____________________________________________________________________________*/﻿

using UnityEngine;
using System.Collections;
using Lambdas;

public class AnimatedValue<T> {

    protected T _value = default(T); // destination value
    protected T _fromValue = default(T); // source value
    protected T _internalValue = default(T); // current value in animation
    protected float animationTime = 0; // time to interpolate
    protected float interpolationParameter = 0; // progress through interpolation
    protected Interpolation<T> interpolation = Lambda.NullInterpolation<T>(); //function for interpolating
    protected MonoBehaviour animator; // component to use for running animations

    /** The "visible" value, set by the animation */
    public T Current {
		get {
            return _internalValue;
        }
	}

    /** The actual logical value */
	public T Value {
		get {
			return _value;
		}
		set {
            _value = value;
            interpolationParameter = 0;
            _fromValue = _internalValue;
            if (animator != null && animator.enabled)
            {
                if (!animating)
                {
                    animating = true;
                    animator.StartCoroutine(AnimateValue());
                }
            } else {
                Debug.Log("No monobehaviour associated with this value to animate!");
                _internalValue = value;
            }
        }
	}

    public float Time {
        get { return animationTime; }
        set { animationTime = value; }
    }

    public Lambdas.Interpolation<T> Interpolation {
        get { return interpolation; }
        set { interpolation = value; }
    }

    public MonoBehaviour Animator {
        get { return animator; }
        set { animator = value; }
    }

    public Lambdas.Setter<T> SetCallback {get; set;}

    public AnimatedValue(T initial) {
        _value = initial;
        _internalValue = initial;
    }

    public AnimatedValue(T initial, Lambdas.Interpolation<T> animation, float time)
    : this(initial)
    {
        interpolation = animation;
        animationTime = time;
    }

    public Coroutine WaitForAnimation() {
        return Animator.WaitFor(() => !animating);
    }


    private bool animating;
    protected IEnumerator AnimateValue() {
        for(;   interpolationParameter<1;
                interpolationParameter += UnityEngine.Time.deltaTime/animationTime) {

            _internalValue = interpolation(_fromValue, _value, interpolationParameter);
            if(SetCallback!=null) SetCallback(_internalValue);
            yield return 1;
        }
        animating = false;
    }

}
