/*____________________________________________________________________________

Vector3i

____________________________________________________________________________*/

using UnityEngine;
using System;

public struct Vector3i : IComparable<Vector3i> {

	public int x, y, z;

	// Constructors

	public Vector3i(int xx, int yy, int zz){
		x=xx; y=yy; z=zz;
	}

	public Vector3i(Vector3 v){
		x = Mathf.RoundToInt(v.x);
		y = Mathf.RoundToInt(v.y);
		z = Mathf.RoundToInt(v.z);
	}

	public Vector3i(float xx, float yy, float zz){
		x = Mathf.RoundToInt(xx);
		y = Mathf.RoundToInt(yy);
		z = Mathf.RoundToInt(zz);
	}

	public Vector3 ToVector3(){
		return new Vector3(x,y,z);
	}

	public override string ToString(){
		return "("+x+", "+y+", "+z+")";
	}

	public static Vector3i RoundDown(Vector3 v) {
		return new Vector3i(v.x-0.5f, v.y-0.5f, v.z-0.5f);
	}

	// Operators

	public static Vector3i operator+ (Vector3i a, Vector3i b){
		return new Vector3i(a.x+b.x, a.y+b.y, a.z+b.z);
	}

	public static Vector3i operator- (Vector3i a, Vector3i b){
		return new Vector3i(a.x-b.x, a.y-b.y, a.z-b.z);
	}

	public static Vector3i operator- (Vector3i a){
		return Vector3i.zero - a;
	}

	public override bool Equals(object o){
		return o is Vector3i && ((Vector3i)o)==this;
	}

	public static bool operator== (Vector3i a, Vector3i b){
		return a.x==b.x && a.y==b.y && a.z==b.z;
	}

	public static bool operator!= (Vector3i a, Vector3i b){
		return !(a==b);
	}

	public int Get30BitZCurveValue() {
		var xBias = x + 512;
		var yBias = y + 512;
		var zBias = z + 512;

		var interleaved = 0;

		for (int i = 0; i < 10; i++) {
			interleaved |=
					  (xBias & 1 << i) << i*2
					| (yBias & 1 << i) << (i*2 + 1)
					| (zBias & 1 << i) << (i*2 + 2);
		}

		return interleaved;
	}

	public int CompareTo(Vector3i other) {
		return Get30BitZCurveValue().CompareTo(other.Get30BitZCurveValue());
	}

	public override int GetHashCode(){
		return Get30BitZCurveValue();
	}

	public int this[int index] {
		get {
			switch (index) {
				case 0: {
					return x;
				}
				case 1: {
					return y;
				}
				case 2: {
					return z;
				}
				default: {
					throw new IndexOutOfRangeException();
				}
			}
		}
		set {
			switch (index) {
				case 0: {
					x = value;
					break;
				}
				case 1: {
					y = value;
					break;
				}
				case 2: {
					z = value;
					break;
				}
				default: {
					throw new IndexOutOfRangeException();
				}
			}
		}
	}

	// Casts

	// explicit to mark loss of precision
	public static explicit operator Vector3i(Vector3 v){
		return new Vector3i(v);
	}

	public static explicit operator Vector3i(Vector2 v){
		return new Vector3i(v);
	}

	public static implicit operator Vector3(Vector3i v){
		return new Vector3(v.x, v.y, v.z);
	}

	public static implicit operator Vector2(Vector3i v){
		return new Vector2(v.x, v.y);
	}

	// Sorting

	public static int SortByDistance(Vector3i a, Vector3i b, Vector3i target){
		float aa = (target.ToVector3() - a.ToVector3()).sqrMagnitude;
		float bb = (target.ToVector3() - b.ToVector3()).sqrMagnitude;
		if(aa < bb){
			return -1;
		}else if(bb < aa){
			return +1;
		}else{
			return 0;
		}
	}

	public static int SortByAltitude(Vector3i a, Vector3i b){
		if(a.y < b.y){
			return -1;
		}else if(b.y < a.y){
			return +1;
		}else{
			return 0;
		}
	}

	public static Vector3i MemberwiseMin(Vector3i a, Vector3i b){
		return new Vector3i(Mathf.Min(a.x,b.x), Mathf.Min(a.y,b.y), Mathf.Min(a.z,b.z));
	}

	public static Vector3i MemberwiseMax(Vector3i a, Vector3i b){
		return new Vector3i(Mathf.Max(a.x,b.x), Mathf.Max(a.y,b.y), Mathf.Max(a.z,b.z));
	}

	// Standard
	public static Vector3i up      = new Vector3i( 0,  1,  0);
	public static Vector3i down    = new Vector3i( 0, -1,  0);
	public static Vector3i right   = new Vector3i( 1,  0,  0);
	public static Vector3i left    = new Vector3i(-1,  0,  0);
	public static Vector3i forward = new Vector3i( 0,  0,  1);
	public static Vector3i back    = new Vector3i( 0,  0, -1);
	public static Vector3i zero    = new Vector3i( 0,  0,  0);
	public static Vector3i one     = new Vector3i( 1,  1,  1);

	public static Vector3i[] orthogonalDirections = {
		Vector3i.forward,
		Vector3i.right,
		Vector3i.back,
		Vector3i.left,
		Vector3i.up,
		Vector3i.down
	};

}
