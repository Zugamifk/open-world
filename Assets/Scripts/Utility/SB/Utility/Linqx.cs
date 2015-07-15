using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Linqx {

	/** Joins two sequences of objects of two types into a sequence sizeof objects of a new type */
	public static IEnumerable<T> Zip<A, B, T>(
		this IEnumerable<A> seqA, IEnumerable<B> seqB, Func<A, B, T> func)
	{
		if (seqA == null) throw new ArgumentNullException("seqA");
		if (seqB == null) throw new ArgumentNullException("seqB");

		using (var iteratorA = seqA.GetEnumerator())
			using (var iteratorB = seqB.GetEnumerator())
		{
			while (iteratorA.MoveNext() && iteratorB.MoveNext())
			{
				yield return func(iteratorA.Current, iteratorB.Current);
			}
		}
	}

	/** Joins two sequences of objects of two types into a sequence sizeof objects of a new type */
	public static void Zip<A, B>(
		this IEnumerable<A> seqA, IEnumerable<B> seqB, Action<A, B> act)
	{
		if (seqA == null) throw new ArgumentNullException("seqA");
		if (seqB == null) throw new ArgumentNullException("seqB");

		using (var iteratorA = seqA.GetEnumerator())
			using (var iteratorB = seqB.GetEnumerator())
		{
			while (iteratorA.MoveNext() && iteratorB.MoveNext())
			{
				act(iteratorA.Current, iteratorB.Current);
			}
		}
	}

	public static void ForEach<T>(
		this IEnumerable<T> seq,
		Action<T> func
	) {
		foreach(T item in seq) {
			func(item);
		}
	}

	// this is identical to Zip above, hurp
	public static void ForEach<T, A>(
		this IEnumerable<T> seqT,
		IEnumerable<A> seqA,
		Action<T, A> func
	) {
		if (seqT == null) throw new ArgumentNullException("seqT");
		if (seqA == null) throw new ArgumentNullException("seqA");

		using (var iteratorT = seqT.GetEnumerator())
			using (var iteratorA = seqA.GetEnumerator())
		{
			while (iteratorT.MoveNext() && iteratorA.MoveNext())
			{
				func(iteratorT.Current, iteratorA.Current);
			}
		}
	}

	public static void ForEach<T>(this IEnumerable<T> seq, Action<T, int> func) {
		int i = 0;
		foreach(var v in seq) func(v, i++);
	}

	public static T Random<T>(
		this IEnumerable<T> seq
	) {
		var max = seq.Count();
		if (max == 0) return default(T);
		int i = UnityEngine.Random.Range(0, max);
		return seq.ElementAt(i);
	}

	public static T Max<T>(
		this IEnumerable<T> seq
	) where T : IComparable<T> {
		return seq.Aggregate(
			(a,b) =>
				a.CompareTo(b) > 0 ?
					a :
					b
			);
	}

	public static T Min<T>(
		this IEnumerable<T> seq
	) where T : IComparable<T> {
		return seq.Aggregate(
			(a,b) =>
				a.CompareTo(b) < 0 ?
					a :
					b
			);
	}

	/** Checks if an IEnumerable<T> contains an item */
	public static bool Contains<T>(
		this IEnumerable<T> seq, T item
	) where T : IEquatable<T> {
        return seq.Any(i => i.Equals(item));
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

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
	{
		return source.Shuffle(new System.Random());
	}

	public static IEnumerable<T> Shuffle<T>(
		this IEnumerable<T> source, System.Random rng)
	{
		if (source == null) throw new ArgumentNullException("source");
		if (rng == null) throw new ArgumentNullException("rng");

		return source.ShuffleIterator(rng);
	}

	private static IEnumerable<T> ShuffleIterator<T>(
		this IEnumerable<T> source, System.Random rng)
	{
		var buffer = source.ToList();
		for (int i = 0; i < buffer.Count; i++)
		{
			int j = rng.Next(i, buffer.Count);
			yield return buffer[j];

			buffer[j] = buffer[i];
		}
	}
}
