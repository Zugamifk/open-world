using UnityEngine;
using System;
using System.Collections;

namespace Lambdas
{
    public delegate T Pure0<T>();
    public delegate T Pure1<T>(T arg);
    public delegate Pure1<T> Pure2<T>(T arg);
    public delegate Pure2<T> Pure3<T>(T arg);
    public delegate Pure3<T> Pure4<T>(T arg);
    public delegate Pure4<T> Pure5<T>(T arg);
    public delegate Pure5<T> Pure6<T>(T arg);
    public delegate Pure6<T> Pure7<T>(T arg);
    public delegate Pure7<T> Pure8<T>(T arg);
    public delegate Pure8<T> Pure9<T>(T arg);

    public static partial class Lambda
    {


		public static Pure1<T> Curried<T>(this Action a) {
            return arg =>
            {
                a();
                return default(T);
            };
        }

		public static Pure1<T> Curried<T>(this Action<T> a) {
            return arg =>
            {
                a(arg);
                return default(T);
            };
        }

		public static Pure2<T> Curried<T>(this Action<T, T> a) {
            return arg1 => arg2 =>
            {
                a(arg1, arg2);
                return default(T);
            };
        }

		public static Pure3<T> Curried<T>(this Action<T,T,T> a) {
            return arg1 => arg2 => arg3 =>
            {
                a(arg1, arg2, arg3);
                return default(T);
            };
        }

		public static Pure1<T> Curried<T>(this Func<T> f) {
            return arg => f();
        }

		public static Pure1<T> Curried<T>(this Func<T, T> f) {
            return new Pure1<T>(f);
        }

		public static Pure2<T> Curried<T>(this Func<T, T, T> f) {
            return arg1 => arg2 => f(arg1, arg2);
        }

		public static Pure3<T> Curried<T>(this Func<T, T, T, T> f) {
            return arg1 => arg2 => arg3 => f(arg1, arg2, arg3);
        }
    }
}
