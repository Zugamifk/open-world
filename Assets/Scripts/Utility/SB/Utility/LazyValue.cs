/*____________________________________________________________________________
Lazy Value

A generic Decorator class that performs lazy evaluation. Its default behaviour
causes it to evaluate the getter function only once, the first time Value is
accessed.
Optionally, you can provide a setter function to control Set behaviour, or
a test function to check if the Value needs to be re-evaluated.
____________________________________________________________________________*/﻿﻿

using UnityEngine;
using System;
using System.Collections;

public class LazyValue<T> {

    Lambdas.Getter<T> getter = Lambdas.NullGetter<T>();
    Lambdas.Setter<T> setter = Lambdas.NullSetter<T>();
    Lambdas.Truth test = Lambdas.True;

    private bool set = false;
    private T internalValue = default(T);
	public T Value {
		get {
            if(!set || !test()) internalValue = getter();
            return internalValue;
        }
		set {
			internalValue = value;
            setter(value);
        }
	}
	public void Evaluate() {
		internalValue = getter();
    }

    public LazyValue() { }

    public LazyValue(Lambdas.Getter<T> getter) {
		this.getter = getter;
	}

    public LazyValue(Lambdas.Getter<T> getter, Lambdas.Setter<T> setter)
	: this(getter) {
		this.setter = setter;
	}

    public LazyValue(Lambdas.Getter<T> getter, Lambdas.Setter<T> setter, Lambdas.Truth test)
    : this(getter, setter) {
        this.test = test;
    }

    public LazyValue(Lambdas.Getter<T> getter, Lambdas.Truth test)
    : this(getter) {
        this.test = test;
    }

}
