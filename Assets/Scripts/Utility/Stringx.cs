using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

public static class Stringx {

	public static string Capitalize(this string str) {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

	public static string Uncamel(this string str) {
		var reg = new Regex(@"
			(?<=[A-Z])(?=[A-Z][a-z]) |
				(?<=[^A-Z])(?=[A-Z]) |
				(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        return reg.Replace(str, " ").Capitalize();
    }
}
