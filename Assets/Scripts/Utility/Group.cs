using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Group<T> {
    public readonly HashSet<T> Closure;
    public readonly Func<T, T, T> Operation;
    public readonly Func<T, T> Inverse;
    public readonly T Identity;


    public Group(HashSet<T> closure, Func<T,T,T> operation, Func<T, T> inverse, T id) {
        this.Closure = closure;
        this.Operation = operation;
        this.Inverse = inverse;
        this.Identity = id;
    }

    /** Checks that a group satisfies the 4 group axioms. Runs in O(n^2). */
    public bool IsValid {
        get
        {
            if (Closure.Contains(Identity))
            {
                Debug.LogError("Group set does not contain its identity!");
                return false;
            }
            foreach (var Gi in Closure)
            {
                if (!Closure.Contains(Inverse(Gi)))
                {
                    Debug.LogError("Group set does not contain the inverse of " + Gi + "!");
                    return false;
                }
                foreach (var Gj in Closure)
                {
                    if (!Closure.Contains(Operation(Gi, Gj)))
                    {
                        Debug.LogError("Group operation is not closed for " + Gi + " * " + Gj +" = " + Operation(Gi, Gj));
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public T Aggregate(params T[] args) {
        T result = Identity;
        foreach(var arg in args) {
            result = Operation(result, arg);
        }
        return result;
    }



}

public static class Group {
    public static Group<int[]> Symmetric(int num) {
        var permutations = new HashSet<int[]>();
        var id = Enumerable.Range(0, num).ToArray();
        foreach(var p in id.Choose(num, false)) {
            permutations.Add(p.ToArray());
        }
        return new Group<int[]>(
            permutations,
            (int[] p0,int[] p1) => p1.Select(p1i => p0[p1i]).ToArray(),
            (int[] p) => {
                var inv = new int[p.Length];
                p.ForEach((pi,i)=>inv[pi]=i);
                return inv;
            },
            id
        );
    }
}
