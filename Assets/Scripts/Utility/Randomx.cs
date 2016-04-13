using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Randomness utility functions */
public static class Randomx {

	/** Returns a random element from an IList*/
	public static T Element<T>(IList<T> list) {
		return list[Index((ICollection<T>)list)];
	}

	/** Returns a random valid index for the given collection */
	public static int Index<T>(ICollection<T> collection) {
		return Random.Range(0, collection.Count);
	}

	/** Returns a random valid index for the given collection */
	public static int Index(ICollection collection) {
		return Random.Range(0, collection.Count);
	}

	/** Returns a random valid index for a collection of the given length */
	public static int Index(int length) {
		return Random.Range(0, length);
	}

	// based on Knuth's algorithm
	public static int[] ChooseWithoutReplacement(int min, int max, int count) {
		count = Mathf.Min(max-min+1, count);
		var res = new int[count];

		int n = 0; // number found so far
		int N = max-min+1; // total number of elements
		int t = 0;
		for(int i=min;i<=max;i++) {
			if(Random.value*(float)(N-t) < (float)(count-n)) {
				res[n] = i;
				n++;
			}
		}

		return res;
	}

	// Get a random point in Bounds object
	public static Vector3 RandomPoint(this Bounds bounds) {
		var point = Vector3.zero;
		for (int i = 0; i < 3; ++i) {
			point[i] = bounds.min[i] + Random.value*bounds.extents[i]*2f;
		}
		return point;
	}

	// Return a normally distributed number with variance 1.0 and median 0;
	public static float NormalDistribution(){
		float sum = 0;
		for(int i = 0; i < 12; i++){
			sum += Random.value;
		}
		sum -= 6.0f;
		return sum;
	}

	// Durstenfeld's in-place Fisher-Yates shuffle
	// Randomizes the order of elements in a list.
	public static void Shuffle<T>(IList<T> array) {
		for (int n = array.Count; n > 1; --n) {
			int k = Index(n);
			T temp = array[n - 1];
			array[n - 1] = array[k];
			array[k] = temp;
		}
	}

	/** Returns the Cartesian coordinates of a random point in a triangle */
	public static Vector2 PointInTriangle(Vector2 pt1, Vector2 pt2, Vector2 pt3){
		float a = Random.value; //a, b, c are barycentric coordinates
		float b = Random.value;
		if(a+b > 1){ //if a+b > 1 we can harmlessly "flip" them and leave room for our C coorindate
			a = 1-a;
			b = 1-b;
		}
		float c = 1-a-b; //c is derived from a and b
		return pt1*a + pt2*b + pt3*c;
	}

	/** Returns the Cartesian coordinates of a random point in a triangle */
	public static Vector3 PointInTriangle(Vector3 pt1, Vector3 pt2, Vector3 pt3){
		float a = Random.value;
		float b = Random.value;
		if(a+b > 1){
			a = 1-a;
			b = 1-b;
		}
		float c = 1-a-b;
		return pt1*a + pt2*b + pt3*c;
	}

	/** Returns a unit vector pointed in a random direction */
	public static Vector3 Direction(){
		float theta = Random.value * 2 * Mathf.PI;
		float rawX = Mathf.Sin(theta);
		float rawY = Mathf.Cos(theta);
		float z = Random.Range(-1f, 1f);
		float phi = Mathf.Asin(z);
		float scalar = Mathf.Cos(phi);
		return new Vector3(rawX * scalar, rawY * scalar, z);
	}

	/** Returns a unit vector pointed in a random direction, using the given RNG */
	public static Vector3 Direction(System.Random rng){
		float theta = (float)(rng.NextDouble() * 2 * Mathf.PI);
		float rawX = Mathf.Sin(theta);
		float rawY = Mathf.Cos(theta);
		float z = (float)(rng.NextDouble() * 2 - 1);
		float phi = Mathf.Asin(z);
		float scalar = Mathf.Cos(phi);
		return new Vector3(rawX * scalar, rawY * scalar, z);
	}

	/** Returns a random rotation **/
	public static Quaternion Rotation(){
		Vector3 dir = Direction();
		float theta = Random.value * 2 * Mathf.PI;
		float cosTheta = Mathf.Cos(theta);
		float sinTheta = Mathf.Sin(theta);
		return new Quaternion(dir.x * sinTheta, dir.y * sinTheta, dir.z * sinTheta, cosTheta);
	}

	/** Return a random unit vector on the XZ plane **/
	public static Vector3 OnXZCircle {
		get {
			var rng = Random.value*2*Mathf.PI;
			return new Vector3(Mathf.Sin(rng), 0, Mathf.Cos(rng));
		}
	}

	/** Return a random unit vector on the XY plane **/
	public static Vector3 OnXYCircle {
		get {
			var rng = Random.value*2*Mathf.PI;
			return new Vector3(Mathf.Sin(rng), Mathf.Cos(rng), 0);
		}
	}

	public static float Sign {
		get {
			return Random.value > 0.5f ? 1 : -1;
		}
	}

	public static float m11 {
		get {
			return Random.value*2-1;
		}
	}

    public static bool Bool
    {
        get
        {
            return Random.value < 0.5f;
        }
    }

}
