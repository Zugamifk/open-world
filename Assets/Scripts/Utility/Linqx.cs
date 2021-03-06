﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Linqx {
    public static System.Type GetEnumeratedType<T>(this IEnumerable<T> seq) {
        return typeof(T);
    }

    // copied From stackoverflow.com/a/13608408/284795
    public static bool TryListOfWhat(Type type, out Type innerType)
    {
        var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) ? i.GetGenericArguments().FirstOrDefault() : null);

        innerType = interfaceTest(type);
        if (innerType != null)
        {
            return true;
        }

        foreach (var i in type.GetInterfaces())
        {
            innerType = interfaceTest(i);
            if (innerType != null)
            {
                return true;
            }
        }

        return false;
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

    public static IEnumerable<T> Cons<T>(this T head, IEnumerable<T> tail) {
      yield return head;
      foreach(var c in tail) {
        yield return c;
      }
    }

    public static T Second<T>(this IEnumerable<T> seq) {
        return seq.ElementAt(1);
    }

    public static T Third<T>(this IEnumerable<T> seq) {
        return seq.ElementAt(2);
    }

    public static T Fourth<T>(this IEnumerable<T> seq) {
        return seq.ElementAt(3);
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(
        this IEnumerable<IEnumerable<T>> seqA,
        IEnumerable<T> seqB) {
        return from a in seqA
               from b in seqB
               select a.Concat(b.Single());
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(
        this IEnumerable<T> seqA,
        IEnumerable<T> seqB) {
        return seqA.Single().Cross(seqB);
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(
        this IEnumerable<IEnumerable<T>> seqA,
        IEnumerable<T> seqB,
        Func<IEnumerable<T>, T, bool> filter) {
        return from a in seqA
               from b in seqB
               where filter(a,b)
               select a.Concat(b.Single());
    }

    public static IEnumerable<IEnumerable<T>> Cross<T>(
        this IEnumerable<T> seqA,
        IEnumerable<T> seqB,
        Func<IEnumerable<T>, T, bool> filter)
    {
        return seqA.Single().Cross(seqB, filter);
    }

    public static object[] ToObjectArray<T>(this IEnumerable<T> seq) {
        var result = new object[seq.Count()];
        seq.ForEach((item, i)=>result[i] = (object)item);
        return result;
    }

    public static IEnumerable<IEnumerable<T>> Choose<T>(this IEnumerable<T> seq, int k, bool replace)
    where T : IComparable {
        if(replace) {
            return Enumerable.Range(0, k).Select(_ => seq)
                .Aggregate(
                    Enumerable.Empty<T>().Single(),
                    (chosen, seqs) => chosen.Cross(seqs)
                );
        } else {
            return Enumerable.Range(0, k).Select(_ => seq)
                .Aggregate(
                    Enumerable.Empty<T>().Single(),
                    (chosen, seqs) =>
                        chosen.Cross(
                            seqs,
                            (subset, val) =>
                                subset.All(
                                    s => s.CompareTo(val)!=0
                                )
                        )
                );
        }
    }

    // From http://community.bartdesmet.net/blogs/bart/archive/2008/11/03/c-4-0-feature-focus-part-3-intermezzo-linq-s-new-zip-operator.aspx
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func) {
        if (first == null)
            throw new ArgumentNullException("first");
        if (second == null)
            throw new ArgumentNullException("second");
        if (func == null)
            throw new ArgumentNullException("func");
        using (var ie1 = first.GetEnumerator())
        using (var ie2 = second.GetEnumerator())
            while (ie1.MoveNext() && ie2.MoveNext())
                yield return func(ie1.Current, ie2.Current);
    }

    public static IEnumerable<KeyValuePair<T, R>> Zip<T, R>(this IEnumerable<T> first, IEnumerable<R> second) {
        return first.Zip(second, (f, s) => new KeyValuePair<T, R>(f, s));
    }

    public static T Random<T>(this IEnumerable<T> seq) {
        return seq.ElementAt(UnityEngine.Random.Range(0,seq.Count()));
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        var r = new System.Random((int)DateTime.Now.Ticks);
        var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
        return shuffledList.ToList();
    }

    public static IEnumerable<T> Generate<T>(Func<T> generator)
    {
        while (true)
        {
            yield return generator();
        }
    }
}
