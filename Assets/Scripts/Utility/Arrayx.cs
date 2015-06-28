using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Arrayx {

	/** Performs a function on each element of an array, or a subsection of it,
	replacing the current values with the result of the function */
	public static T[] ToEach<T>(this T[] array, Func<T,T> func, int start = 0, int count = 0) {
        count = count > 0 ? count : array.Length;
        for(int i=start; i<start+count; i++) {
			array[i] = func(array[i]);
    	}
		return array;
	}

	public static T[] ToEach<T>(this T[] array, Func<T, int, T> func, int start = 0, int count = 0) {
		count = count > 0 ? count : array.Length;
		for(int i=start; i<start+count; i++) {
			array[i] = func(array[i], i);
		}
		return array;
	}

    /** Fills an array, or part of it, with the results of repeatedly applying a
	function to some other array */
    public static T[] Fill<T, S>(this T[] array, S[] other, Func<S, T> func, int start = 0, int count = 0) {
		count = count > 0 ? count : array.Length;
        int otherLen = other.Length;
        for(int i=start; i<start+count; i++) {
			array[i] = func(other[(i-start)%otherLen]);
    	}
		return array;
	}

	/** Fills an array, or part of it, with the results of repeatedly applying a
	function to some other array */
	public static T[] Fill<T, S>(this T[] array, S[] other, Func<S, int, T> func, int start = 0, int count = 0) {
		count = count > 0 ? count : array.Length;
		int otherLen = other.Length;
		for(int i=start; i<start+count; i++) {
			array[i] = func(other[(i-start)%otherLen], i-start);
		}
		return array;
	}

    /** Get a subarray of an array */
    public static T[] SubArray<T>(this T[] array, int start = 0, int count = 0) {
		count = count > 0 ? count : array.Length;
        T[] sub = new T[count];
        for(int i=0; i<count; i++) {
			sub[i] = array[start+i];
    	}
		return sub;
	}

	public static IEnumerable<T[]> Permutations<T>(this T[] arr) {
        T[] result = new T[arr.Length];
		arr.CopyTo(result, 0);
        return PermutationGenerator(arr.Length, result);
    }

	private static IEnumerable<T[]> PermutationGenerator<T>(int N, T[] r)
	{
		if (N == 1)
		{
			yield return r;
		} else {
			for(int i=0;i<N;i++) {
				foreach (var p in PermutationGenerator(N - 1, r)) {
					yield return p;
				}
				if(N%2==0) {
					var val = r[N-1];
					r[N-1] = r[i];
					r[i] = val;
				} else {
					var val = r[0];
					r[0] = r[N - 1];
					r[N - 1] = val;
				}
			}
		}
	}
}
