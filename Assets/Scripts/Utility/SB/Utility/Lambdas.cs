/*____________________________________________________________________________
Lambdas

A static class containing general use lambdas, for functional stuff
____________________________________________________________________________*/﻿
﻿using UnityEngine;
using System.Collections;

public static class Lambdas {

    //___________________________________________________/   Delegate Types \___

    // ACTIONS
    public static System.Action Null = () => { };
    public static System.Action Log(System.Func<string> stringFunc) {
        return () => Debug.Log(stringFunc());
    }

    // TRUTH
    /** A bool function type, for tests */
    public delegate bool Truth();

	/** Always returns true */
	public static Truth True = () => true;
	/** Always returns false */
	public static Truth False = () => false;

    // VALUE
	/** A generic function for getting values */
    public delegate T Getter<T>();
	/** A generic function for setting values */
    public delegate void Setter<T>(T value);


	/** Return a generic default getter that returns default(T) */
    public static Getter<T> NullGetter<T>()
    {
        return () => default(T);
    }

	/** Returns a generic default setter that does nothing */
	public static Setter<T> NullSetter<T>()
    {
        return t => { };
    }

	/** A generic function for interpolating between values */
	public delegate T Interpolation<T>(T from, T to, float param);

	public static Interpolation<T> NullInterpolation<T>() {
        return (_0, _1, _2) => default(T);
    }

}
