using UnityEngine;
using System;
using System.Collections;

namespace Lambdas
{
    public static partial class Lambda
    {

        public delegate T Function<T>(T arg);

		public static Function<T> Pure<T>(Action a) {
            return arg =>
            {
                a();
                return default(T);
            };
        }

		public static Function<T> Pure<T>(Action<T> a) {
            return arg =>
            {
                a(arg);
                return default(T);
            };
        }

		public static Func<T, Function<T>> Pure<T>(Action<T, T> a) {
            return arg1 => arg2 =>
            {
                a(arg1, arg2);
                return default(T);
            };
        }

		public static Func<T, Func<T, Function<T>>> Pure<T>(Action<T,T,T> a) {
            return arg1 => arg2 => arg3 =>
            {
                a(arg1, arg2, arg3);
                return default(T);
            };
        }

		public static Function<T> Pure<T>(Func<T> f) {
            return arg => f();
        }

		public static Function<T> Pure<T>(Func<T, T> f) {
            return new Function<T>(f);
        }

		public static Func<T, Function<T>> Pure<T>(Func<T, T, T> f) {
            return arg1 => arg2 => f(arg1, arg2);
        }

		public static Func<T, Func<T, Function<T>>> Pure<T>(Func<T, T, T, T> f) {
            return arg1 => arg2 => arg3 => f(arg1, arg2, arg3);
        }
    }
}
