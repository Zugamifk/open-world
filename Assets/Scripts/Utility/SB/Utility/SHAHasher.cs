using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;

/** String hasher / hash verifier using SHA.
	Based on the example code from the MSDN artcle on HashAlgorithm.ComputeHash() */

public class SHAHasher {
	
	public static string GetHash(string[] input){
		return GetHash(input, 0, input.Length);
	}
	
	// Hash an input string array and return the hash as
	// a hexadecimal string.
	public static string GetHash(string[] input, int start, int length)
	{
		SHA256Managed hasher = new SHA256Managed();
		if(hasher == null){
			Debug.Log("Could not create hasher");
			return "0";
		}
		
		// Convert the input string[] to a byte array 
		byte[][] inputLinesAsBytes = new byte[length][];
		int totalBytes = 0;
		UTF8Encoding encoder = new UTF8Encoding();
		for(int i = start; i < start + length; i++){
			inputLinesAsBytes[i] = encoder.GetBytes(input[i]);
			totalBytes += inputLinesAsBytes[i].Length;
		}
		byte[] inputAsBytes = new byte[totalBytes];
		int index = 0;
		for(int i = 0; i < inputLinesAsBytes.Length; i++){
			for(int j = 0; j < inputLinesAsBytes[i].Length; j++){
				inputAsBytes[index] = inputLinesAsBytes[i][j];
				index++;
			}
		}
		
		// compute the hash
		byte[] data = hasher.ComputeHash(inputAsBytes);

		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}
	
	public static string GetHash(byte[] input){
		return GetHash(input, 0, input.Length);
	}
	
	public static string GetHash(byte[] input, int start, int length){
		SHA256Managed hasher = new SHA256Managed();
		if(hasher == null){
			Debug.Log("Could not create hasher");
			return "0";
		}
		byte[] hashBytes = hasher.ComputeHash(input, start, length);
		
		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();

		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < hashBytes.Length; i++)	{
			sBuilder.Append(hashBytes[i].ToString("x2"));
		}

		// Return the hexadecimal string.
		return sBuilder.ToString();
	}

	public static bool VerifyHash(string[] input, string hash){
		return VerifyHash(input, 0, input.Length, hash);
	}

	// Verify a hash against a string array.
	public static bool VerifyHash(string[] input, int start, int length, string hash) {
		// Hash the input.
		string hashOfInput = GetHash(input, start, length);

		// Create a StringComparer an compare the hashes.
		StringComparer comparer = StringComparer.OrdinalIgnoreCase;

		if (0 == comparer.Compare(hashOfInput, hash))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
