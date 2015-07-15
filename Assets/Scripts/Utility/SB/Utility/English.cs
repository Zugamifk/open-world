using UnityEngine;
using System;
using System.Globalization;

/** Utility string-handling class */
public static class English {
	/* Compares two strings lexicographically, but numerical substrings are parsed to sort them in ascending order.
		Example: file100 is lexicographically before file99, but this function parses the "99" and "100" as
		numbers rather than their constituent digits and will put file100 after file99.
	 */
	public static int NaturalCompareTo(this string a, string b) {
		int aIndex = 0,
			bIndex = 0;
		while (aIndex < a.Length && bIndex < b.Length) {
			if (Char.IsDigit(a[aIndex]) && Char.IsDigit(b[bIndex])) {
				string aNumber;
				string bNumber;
				int aDigits = 0;
				int bDigits = 0;
				while (aIndex + aDigits < a.Length && Char.IsDigit(a[aIndex + aDigits])) {
					++aDigits;
				}
				while (bIndex + bDigits < b.Length && Char.IsDigit(b[bIndex + bDigits])) {
					++bDigits;
				}
				aNumber = a.Substring(aIndex, aDigits);
				bNumber = b.Substring(bIndex, bDigits);
				aIndex += aDigits;
				bIndex += bDigits;
				int order =
					Int32.Parse(
						aNumber,
						CultureInfo.InvariantCulture
					).CompareTo(
						Int32.Parse(
							bNumber,
							CultureInfo.InvariantCulture
						)
					)
				;
				if (order == 0) {
					continue;
				} else {
					return order;
				}
			} else if (a[aIndex] == b[bIndex]) {
				++aIndex;
				++bIndex;
			} else {
				return a[aIndex].CompareTo(b[bIndex]);
			}
		}
		return a.Length.CompareTo(b.Length);
	}
	
}