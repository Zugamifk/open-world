using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Linqx {
    public static System.Type GetEnumeratedType<T>(this IEnumerable<T> seq) {
        return typeof(T);
    }

    public static void ForEach<T>(this IEnumerable<T> seq, Action<T> func) {
        foreach(var v in seq) func(v);
    }

    public static void ForEach<T>(this IEnumerable<T> seq, Action<T, int> func) {
        int i = 0;
        foreach(var v in seq) func(v, i++);
    }

    public static IEnumerable<T> Single<T>(this T t) {
        yield return t;
    }

    public static IEnumerable<T> With<T>(this T t, params T[] others) {
        yield return t;
        foreach(var o in others) {
            yield return o;
        }
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(this IEnumerable<T> seqA, IEnumerable<T> seqB) {
        return from a in seqA
               from b in seqB
               select a.With(b);
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(this IEnumerable<IEnumerable<T>> seqA, IEnumerable<T> seqB) {
        return from a in seqA
               from b in seqB
               select a.Concat(b.Single());
    }

    public static object[] ToObjectArray<T>(this IEnumerable<T> seq) {
        var result = new object[seq.Count()];
        seq.ForEach((item, i)=>result[i] = (object)item);
        return result;
    }
}
