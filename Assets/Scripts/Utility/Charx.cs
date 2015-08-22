using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Charx {

	public static IEnumerable<char> LowerCase {
		get {
            return Enumerable.Range('a', 'z' - 'a' + 1).Cast<char>();
        }
	}

	public static IEnumerable<char> UpperCase {
		get {
            return Enumerable.Range('A', 'Z' - 'A' + 1).Cast<char>();
        }
	}

	public static bool IsUpper(this char c) {
        return c >= 'A' && c <= 'Z';
    }

	public static bool IsLower(this char c) {
        return c >= 'a' && c <= 'z';
    }
}
