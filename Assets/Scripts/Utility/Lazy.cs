using UnityEngine;
using System;
using System.Collections;

public class Lazy<T> {
    private Func<T> evaluator;
    private bool set = false;
    private T result = default(T);
    public T Value {
		get {
            if (!set) {
                result = evaluator();
                set = true;
            }
            return result;
        }
	}
	public Lazy(Func<T> eval) {
		evaluator = eval;
	}
	public static implicit operator Lazy<T>(Func<T> fun) {
        return new Lazy<T>(fun);
    }
	public static explicit operator Func<T>(Lazy<T> lazy) {
        return lazy.evaluator;
    }
}
