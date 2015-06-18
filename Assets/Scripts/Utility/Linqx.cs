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
}
