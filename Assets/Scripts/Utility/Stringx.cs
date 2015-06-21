using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	public static string Format(this string format, params string[] args) {
        return string.Format(format, args);
    }

	public static int CountFormatArgs(this string format) {
        int max = 0;
        var reg = new Regex(@"(?<=\{)(.*?)(?=\})");
		foreach(Match m in reg.Matches(format)) {
            max = Mathf.Max(max, int.Parse(m.Value));
        }
        return max;
    }

	public static IEnumerable<string> EnumerableFormat(this string format, params IEnumerable<object>[] objects) {
        return objects.ToList().Aggregate(
            Enumerable.Empty<object>().Single(),
            (sets, list) => sets.Cross(list),
            sets => sets.Select(
                args => format.Format(
					args.Select(a=>a.ToString()).ToArray()
				)
            )
        );
    }
}
