using UnityEngine;
using System.Collections;

/** Utility class with string functions */
public static class Stringx {
	/** Returns a copy of the input string with "escaped" characters converted
		into actually escaped characters. E.g. given the string "\n" which has
		two characters ('\' and 'n') this will return the string which has the
		single character '\n'. */
	public static string InterpretEscapeStrings(string s){
		char[] chars = s.ToCharArray();
		char[] result = new char[chars.Length];
		bool escapeNext = false;
		int resultIndex = 0;
		for(int i = 0; i < chars.Length; i++){
			if(escapeNext){
				escapeNext = false;
				switch(chars[i]){
					case 'n':
						result[resultIndex] = '\n';
					break;
					case 't':
						result[resultIndex] = '\t';
					break;
					case '\\':
						result[resultIndex] = '\\';
					break;
					case '\'':
						result[resultIndex] = '\'';
					break;
					case '\"':
						result[resultIndex] = '\"';
					break;
					default:
						result[resultIndex] = '?';
					break;
				}
				resultIndex++;
			}
			else if(chars[i] == '\\') {
				escapeNext = true;
			}
			else{
				result[resultIndex] = chars[i];
				resultIndex++;
			}
		}
		return new string(result, 0, resultIndex);
	}
}
