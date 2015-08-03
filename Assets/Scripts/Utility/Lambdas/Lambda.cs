using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lambdas
{
    /** A bool function type, for tests */
    public delegate bool Truth();

    /** A generic function for getting values */
    public delegate T Getter<T>();
    /** A generic function for setting values */
    public delegate void Setter<T>(T value);

    /** A generic function for interpolating between values */
    public delegate T Interpolation<T>(T from, T to, float param);

    public static partial class Lambda
    {
        //___________________________________________________/   Delegate Types \___

        // ACTIONS
        public static System.Action Null = () => { };
        public static System.Action Log(System.Func<string> stringFunc) {
            return () => Debug.Log(stringFunc());
        }

        // TRUTH
    	/** Always returns true */
    	public static Truth True = () => true;
    	/** Always returns false */
    	public static Truth False = () => false;

        // VALUE

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

    	public static Interpolation<T> NullInterpolation<T>() {
            return (_0, _1, _2) => default(T);
        }

        public static bool NotNull<T>(T obj) where T : IComparable {
            return obj.CompareTo(default(T))!=0;
        }

        public static Func<Ta, Tr> Memoized<Ta, Tr>(this Func<Ta, Tr> fun) {
            Dictionary<Ta, Tr> memoized = new Dictionary<Ta, Tr>();
            return (Ta arg) =>
            {
                Tr result;
                if (!memoized.TryGetValue(arg, out result))
                {
                    result = fun(arg);
                    memoized[arg] = result;
                }
                return result;
            };
        }

        public static Func<Ta, Tr> ObserveCalls<Ta, Tr>(this Func<Ta, Tr> fun, Action<Tr> observer) {
            return arg =>
            {
                var result = fun(arg);
                observer(result);
                return result;
            };
        }
    }
}
