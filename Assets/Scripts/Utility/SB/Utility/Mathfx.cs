using UnityEngine;
using System.Collections.Generic;
using System;

/** Utility class for vector, interpolation, and basic general math functions. */
public static class Mathfx {

	/* A name signature for interpolation function, useful for animations */
	public delegate float ErpFunc(float s, float f, float v);

	/* A name signature for a parametric curve, should be normalized to interpolate from 0 to 1 */
	public delegate float ParametricCurve(float t);

	/* A probability distribution, should generally have an area of 1 under the curve, can be sampled from anywhere */
	public delegate float DistributionCurve(float t);

	// i went a little crazy with this naming convention, sorry -pat
	[System.Serializable]
	public class ErpCurve {
		[SerializeField] AnimationCurve curve;
		private ParametricCurve pCurve;

		public ErpCurve(AnimationCurve c) {
			curve=c;
			pCurve = curve.Evaluate;
		}

		public ErpCurve(ParametricCurve c) {
			pCurve = c;
		}

		public float Evaluate(float param) {
			return pCurve(param);
		}

		public float Erp(float start, float end, float param) {
			return ClamplessLerp(start, end, pCurve(param));
		}

		public ErpFunc ErpFromSegment(float start, float end) {
			return (s, e, t) => ClamplessLerp(s, e, pCurve(ClamplessLerp(start, end, t)));
		}

	}

	//Returns a new Vector3 with each component Abs'd
	public static Vector3 Abs(this Vector3 v) {
		v.x = Mathf.Abs(v.x);
		v.y = Mathf.Abs(v.y);
		v.z = Mathf.Abs(v.z);
		return v;
	}

	/**Returns a vector perpendicular to a given one (RH)**/
	public static Vector2 Perpendicular(this Vector2 v) {
		return new Vector2(-v.y, v.x).normalized;
	}
	/**Returns a vector perpendicular to a given one (RH)**/
	public static Vector3 Perpendicular(this Vector3 v) {
		return new Vector3 (-v.y, v.x, v.z).normalized;
	}

	public static Vector2 InverseScale(this Vector2 v, Vector2 invScaler){
		return new Vector2(v.x / invScaler.x, v.y / invScaler.y);
	}

	//Returns a new Vector3 with each component divided by
	//the corresponding component of the inverse scaling vector.
	public static Vector3 InverseScale(this Vector3 v, Vector3 invScaler){
		return new Vector3(v.x / invScaler.x, v.y / invScaler.y, v.z / invScaler.z);
	}

	public static Vector4 InverseScale(this Vector4 v, Vector4 invScaler){
		return new Vector4(v.x / invScaler.x, v.y / invScaler.y, v.z / invScaler.z, v.w / invScaler.w);
	}

	//Smoothstep between 0 and 1, usually for creating interpolation parameters
	public static float SmoothStep01(float t){
		return t*t*(3 - 2*t);
	}

	/** Return the binomial coefficient nCk in O(k) time*/
	public static int Binom(int n, int k) {
		Func<int, int> formula = null;
		formula = (i) => i>0 ? formula(i-1)*((n+1-i)/i) : 1;
		return formula(k);
	}

	/** Cubic Hermite interpolation function
		Interpolates between a and b by t. The slope at the endpoints of the
		interpolation are given by dadt and dbdt. E.g. dadt = dbdt = 1 gives
		a linear interpolation, dadt = dbdt = 0 gives an ease-in ease-out interpolation.
	*/
	public static float Cerp(float a, float b, float dadt, float dbdt, float t) {
		float	t2 = t*t,
				t3 = t2*t;
		return	a*(2*t3 - 3*t2 + 1f) +
				b*(3*t2 - 2*t3) +
				dadt*(t3 - 2*t2 + t) +
				dbdt*(t3 - t2);
	}

	/** Cubic Hermite interpolation function, fixed endpoint
		See Cerp(), with a = 0 and b = 1. This is a bit faster than Cerp()
		since the interpolation equation can be simplified.
	*/
	public static float Cerp01(float d0, float d1, float t) {
		float	t2 = t*t,
				t3 = t2*t;
		//The same formula as Cerp() but simplified since the endpoints are known to be 0 and 1
		return 	(3*t2 - 2*t3) +
				d0*(t3 - 2*t2 + t) +
				d1*(t3 - t2);
	}

	/** Returns the value of a gaussian distribution with a given median and variance at point x.*/
	public static float Gaussian(float x, float median, float variance){
		float c = Mathf.Sqrt(variance);
		float factor = 1.0f/(c*Mathf.Sqrt(2.0f*Mathf.PI));

		float numerator = -1.0f * (x-median) * (x-median);
		float denominator = (2.0f * c*c);
		return factor*Mathf.Exp(numerator/denominator);
	}

	/** Lerp between two Rects */
	public static Rect Lerp(Rect a, Rect b, float t) {
		return new Rect(
			Mathf.Lerp(a.x, b.x, t),
			Mathf.Lerp(a.y, b.y, t),
			Mathf.Lerp(a.width, b.width, t),
			Mathf.Lerp(a.height, b.height, t)
		);
	}

	/** Add two Rects together by choosing min and max values */
	public static Rect Add(Rect a, Rect b) {
		return Rect.MinMaxRect(
			Mathf.Min(a.xMin, b.xMin),
			Mathf.Min(a.yMin, b.yMin),
			Mathf.Max(a.xMax, b.xMax),
			Mathf.Max(a.yMax, b.yMax)
		);
	}

	/** Lerp without Clamping */
	public static float ClamplessLerp(float a, float b, float t){
		return (b-a)*t + a;
	}

	public static Vector2 ClamplessLerp(Vector2 a, Vector2 b, float t){
		return (b-a)*t + a;
	}

	public static Vector3 ClamplessLerp(Vector3 a, Vector3 b, float t){
		return (b-a)*t + a;
	}


	/** Inverse Lerp without Clamping */
	public static float ClamplessInverseLerp(float a, float b, float t){
		if(a==b){
			Debug.LogError("Can't inverse lerp between " + a +" and " + b);
			return 0;
		}
		return (t-a)/(b-a);
	}


	// apply interpolation functions to vectors
	public static Vector2 Interpolate(Vector2 start, Vector2 end, float value, ErpFunc f) {
		return new Vector2 (f(start.x, end.x, value),
		                    f(start.y, end.y, value));
	}
    public static Vector2 Interpolate(Vector2 start, Vector2 end, float value)
    {
        return Interpolate(start, end, value, Mathf.Lerp);
    }

    public static Vector3 Interpolate(Vector3 start, Vector3 end, float value, ErpFunc f) {
		var res = new Vector3 (f(start.x, end.x, value),
		                       f(start.y, end.y, value),
		                       f(start.z, end.z, value));
//		Debug.Log (start+"\n"+res+"\n"+end);
		return res;
	}
	public static Vector3 Interpolate(Vector3 start, Vector3 end, float value)
    {
        return Interpolate(start, end, value, Mathf.Lerp);
    }

	public static Vector4 Interpolate(Vector4 start, Vector4 end, float value, ErpFunc f) {
		return new Vector4 (f(start.x, end.x, value),
		                    f(start.y, end.y, value),
		                    f(start.z, end.z, value),
		                    f(start.w, end.w, value));
	}
	public static Vector4 Interpolate(Vector4 start, Vector4 end, float value)
    {
        return Interpolate(start, end, value, Mathf.Lerp);
    }

	// apply interpolation functions to interpolation functions WHOOOAOAO
	public static ErpFunc Interpolate(ErpFunc start, ErpFunc end, float value, ErpFunc func) {
		return (s,f,v) => func(start(s,f,v), end(s,f,v), value);
	}

	//are two vectors nearly the same?
	public static bool Approximately(Vector3 a, Vector3 b) {
		Vector3 difference = a - b;
		return difference.sqrMagnitude < Mathf.Epsilon*Mathf.Epsilon;
	}

	/** Returns the smallest angle between two bearings (in degrees) */
	public static float ShortestAngle(float a, float b) {
		return NormalizeAngle(b - a);
	}

	/** Given an angle in degrees, returns that angle normalized
		such that it is in the range [-180, 180] */
	public static float NormalizeAngle(float angle) {
		angle %= 360f;
		if (angle < -180f)
			angle += 360f;
		else if (angle > 180f)
			angle -= 360f;
		return angle;
	}

	/** Given an angle in degrees, returns that angle normalized
		such that it is in the range [0, 360) */
	public static float NormalizeAngle360(float angle){
		angle %= 360f;
		if(angle < 0)
			angle += 360;
		return angle;
	}

	/** Given an angle in degrees, returns that angle clamped to the range [min, max]
		going clockwise from min. Angle is normalized to the range [-180, 180]. */
	public static float ClampAngle (float angle, float min, float max) {
		max = NormalizeAngle(max);
		min = NormalizeAngle(min);
		if (max < min)
			max += 360f;
		float mean = (min + max)*0.5f;
		min -= mean;
		max -= mean;
		angle -= mean;
		angle = NormalizeAngle(angle);
		angle = Mathf.Clamp(angle, min, max) + mean;
		return NormalizeAngle(angle);
	}

	/** Similar to Mathf.Repeat(), but for integers. Essentially the modulo operator,
	 	but with a different behaviour on negative inputs.
		e.g. modulus 3:
		Input	-4	-3	-2	-1	0	1	2	3	4
		Modulo	-1	 0	-2	-1	0	1	2	0	1
		Repeat	 2	 0	 1	 2	0	1	2	0	1
	*/
	public static int Repeat(int t, int modulus){
		modulus = Mathf.Abs(modulus);
		if(t >= 0){
			return t % modulus;
		}
		else{
			int r = t % modulus;
			if(r == 0){
				return 0;
			}
			else{
				return r + modulus;
			}
		}
	}

	/** Returns a point on a Quadratic 2d bezier curve derived from 3 points and
	 	an interpolation value. t should probably be kept in the range [0, 1].
	*/
	public static Vector2 Bezier(Vector2 pt1, Vector2 pt2, Vector2 pt3, float t){
		float c1 = (1-t)*(1-t);
		float c2 = 2*t*(1-t);
		float c3 = t*t;
		return pt1 * c1 + pt2 * c2 + pt3 * c3;
	}

	/** 2d quadratic Bezier curve with 2nd control point derived from endpoints
		and a "swing" factor. Positive swing moves to the right relative to the
		direction of travel as t increases, negative swing moves to the left. */
	public static Vector2 Swing(Vector2 pt1, Vector2 pt3, float swing, float t){
		Vector2 midPointOffset = (pt3 - pt1)/2; //Get relative midpoint of p1->p3
		Vector2 swingVec = new Vector2(-midPointOffset.y, midPointOffset.x); //Make a vector perpendicular to p1->p2
		Vector2 pt2 = pt1 + midPointOffset + swingVec * swing;
		return Bezier(pt1, pt2, pt3, t);
	}

	/** Returns area of the triangle defined by the 3 points. */
	public static float TriangleArea(Vector3 pt1, Vector3 pt2, Vector3 pt3){
		return Vector3.Cross((pt2-pt1), (pt3-pt1)).magnitude/2;
	}

	/** lerps from 1 to 0 */
	public static float Unlerp(float s, float f, float v) {
		return f + (s-f)*v;
	}

	/** Subset of Mathfx.cs from http://www.unifycommunity.com/wiki/index.php?title=Mathfx#C.23_-_Mathfx.cs
		"Interpolates" between start and end. Overshoots by about 10% by t ~= 0.6, then wavers around the end value before coming to rest.
		The end result is a "bouncy" interpolation.
	*/
	public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

	public static float MyBerp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = 1 - (Mathf.Cos(value * Mathf.PI * 4)*.5f + 0.5f)*(1-value);
		return start + (end - start) * value;
	}

	/** 0:dx=1 -- 1:dx=0 */
	public static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

	/** 0:dx=0 -- 1:dx=0 */
	public static float SinerpIO(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 0.5f-0.5f*Mathf.Cos(value * Mathf.PI));
	}

	/** 0:dx=0 -- 1:dx=1 */
	public static float SinerpI(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1-Mathf.Cos(value * Mathf.PI*0.5f));
	}

	/** f(t) = t*t */
	public static float QuadErp(float start, float end, float value) {
		return Mathf.Lerp(start, end, value*value);
	}

	public static float CubicErp(float start, float end, float value) {
		return Mathf.Lerp(start, end, value*value*value);
	}

	/** f(t) = (1-t)*(1-t) */
	public static float QuadErpO(float start, float end, float value) {
		return Mathf.Lerp(start, end, (1-value)*(1-value));
	}

	public static float PowErp(float start, float end, float value, float pow) {
		return Mathf.Lerp(start, end, Mathf.Pow(value, pow));
	}

	// bouncy interpolation with variable bumps
	private const float bumps = 2;
	public static float Hberp(float start, float end, float value) {
		var res = Mathf.Clamp01(value);
		return Mathf.Lerp (start, end, 1-Mathf.Abs(Mathf.Cos(value*value*Mathf.PI*bumps))*(1-res));
	}

	public static ErpFunc WibbleWobbleErp(float wibbles, float wobbles ) {
		return (s,f,v) => Mathf.Lerp(s,f,v)+SinN(v*wobbles)*wibbles*v*(1-v);
	}

	/** doesn't really interpolate, just wobbles */
	public static ErpFunc WobbleErp(float m) {
		return (s,f,v) => ClamplessLerp(s,f,SinN(v*m));
	}

	/** reverses interpolatio arguments in the function, goo for interpolating backwards*/
	public static ErpFunc ReverseErp(ErpFunc func) {
		return (s,f,v) => func(f,s,v);
	}

	/** Scales results of an interpolation */
	public static ErpFunc ScaleErp(ErpFunc func, float scale) {
		return (s, f, v) => ClamplessLerp(s,f,func(0,scale,v));
	}

	/** takes two interpolation functions and creates a new one which evaluates both and returns the products of their results. Useful for modulation. */
	public static ErpFunc MulErp(ErpFunc a, ErpFunc b) {
		return (s,f,v) => a(s,f,v)*b(s,f,v);
	}

	/** takes two interpolation functions and returns a new one which is the result of adding the results of the two */
	public static ErpFunc AddErp(ErpFunc a, ErpFunc b) {
		return (s,f,v) => a(s,f,v)+b(s,f,v);
	}

	/** Raises the result of an interpolation to a power, generally increases sharpness of turns and steepness of slopes */
	public static ErpFunc ExpErp(ErpFunc func, float power) {
		return (s,f,v) => Mathf.Pow(func(s,f,v), power);
	}

	/** Composes two interpoaltions A and B by applying B to the interpolating parameter and using the result to interpolat with A as follows: a(start, end, b(0, 1, param)) */
	public static ErpFunc CompErp(ErpFunc a, ErpFunc b) {
		return (s,f,v) => a(s,f,b(0,1,v));
	}

	/** Changes interval values */
	public static ErpFunc SubInterval(ErpFunc func, float start, float end) {
        return (s, f, v) => Mathfx.ClamplessLerp(start, end, func(s, f, v));
    }

	/** Takes an interpolation and returns a new one conatining the given one repeated a number of time */
	public static ErpFunc RepeatingErp(ErpFunc func, float repeats) {
		return (s,f,v) => func(s,f,(v*repeats)%1f);
	}

	/** Turns a parametric, single-argument funciton into an interpolation fucniton*/
	public static ErpFunc ErpFromPCurve(ParametricCurve c) {
		return (s,f,v)=>Mathfx.ClamplessLerp(s,f,c(v));
	}

	/** Creates an interpolation function by sticking different curves together */
	public static ErpFunc Piecewise(ErpFunc a, ErpFunc b, float seam) {
		return (s,f,v) => v<seam?
			a(s, a(s,f,seam), v/seam) :
			b(a(s,f,seam), f, (v-seam)/(1-seam));
	}

	public static ErpFunc NoiseOnErp(ErpFunc fun, float noiseLevel) {
		return (s,f,v) => fun(s,f,v) + UnityEngine.Random.value*noiseLevel;
	}

	/** Generates random number between 0 and 1 */
	public static float Noise01(float s, float f, float v) {
		return UnityEngine.Random.value;
	}

	/** Generates random numbers between 1 and -1 */
	public static float Noise11(float s, float f, float v) {
		return UnityEngine.Random.value*2-1;
	}

	public static Matrix4x4 Interpolate4x4(Matrix4x4 start, Matrix4x4 end, float value, ErpFunc f) {
		Matrix4x4 mat = new Matrix4x4();
		mat.SetRow(0, Interpolate(start.GetRow(0), end.GetRow(0), value, f));
		mat.SetRow(1, Interpolate(start.GetRow(1), end.GetRow(1), value, f));
		mat.SetRow(2, Interpolate(start.GetRow(2), end.GetRow(2), value, f));
		mat.SetRow(3, Interpolate(start.GetRow(3), end.GetRow(3), value, f));
		return mat;
	}

	public static ErpFunc ConstErp(float num) {
		return (s,f,v) => num;
	}

	public static ErpFunc To11(ErpFunc func) {
		return (s,f,v) => 2*func(s,f,v) -1;
	}

	public static ErpFunc From11(ErpFunc func) {
		return (s,f,v) => 0.5f*(func(s,f,v)+1);
	}

	public static float Const0(float start, float end, float val) {
		return 0;
	}

	public static float Const1(float start, float end, float val) {
		return 1;
	}


	public static ErpFunc PowErpI(float power) {
		return (s,f,v) => Mathf.Lerp(s,f,Mathf.Pow(v, power));
	}

	public static ErpFunc PowErpO(float power) {
		return (s,f,v) => Mathf.Lerp(s,f,1-Mathf.Pow(1-v, power));
	}

	public static float PingPongErp(float start, float end, float val) {
		val*=2;
		return Mathf.Lerp(start, end, val>1?2-val:val);
	}

	// returns a funciton which pingpongs back and forth using hte given interpolation
	public static ErpFunc ErpToOscillation(ErpFunc fun) {
		return (s,f,v) => v*2<1 ?
							fun(s,f,v*2) :
							fun(f,s,v*2-1);
	}

	public static ParametricCurve LogisticCurve(float steepness) {
		return t=>1/(1+Mathf.Exp(-steepness*t));
	}

	public static float NormalDistribution(float x) {
		return Mathf.Exp (-0.5f*x*x)/Mathf.Sqrt (2*Mathf.PI);
	}

	public static float RandomUIntToFloat(uint i) {
		return (((float)i)%1000)/1000f;
	}

	public static IEnumerable<int> PowersOf2() {
		int i = 1;
		while(true) {
			yield return i;
			i *= 2;
		}
	}

	public static IEnumerable<int> PowersOf(int n) {
		int i = 1;
		while(true) {
			yield return i;
			i *= n;
		}
	}

	// Trig functions with normalized args
	/** Sin function with an interval of 0-1 rather than 0-Pi */
	public static float SinN(float x) {
		return Mathf.Sin(x*2.0f*Mathf.PI);
	}
	/** Cos function with an interval of 0-1 rather than 0-Pi */
	public static float CosN(float x) {
		return Mathf.Cos(x*2.0f*Mathf.PI);
	}
	// Trig functions ranging between [0,1]
	/** Sin function with a range of 0-1 */
	public static float SinU(float x) {
		return 0.5f+0.5f*Mathf.Sin(x);
	}
	/** Cos function with a range of 0-1 */
	public static float CosU(float x) {
		return 0.5f+0.5f*Mathf.Cos(x);
	}
	// As above but with normalized args
	/** Sin function with an interval of 0-1 and a range of 0-1 */
	public static float SinNU(float x) {
		return 0.5f+0.5f*Mathf.Sin(x*2.0f*Mathf.PI);
	}
	/** Cos function with an interval of 0-1 and a range of 0-1 */
	public static float CosNU(float x) {
		return 0.5f+0.5f*Mathf.Cos(x*2.0f*Mathf.PI);
	}

	//Hyperbolic Functions
	/** Hyperbolic Sine*/
	public static float Sinh(float x) {
		return (Mathf.Exp (x)-Mathf.Exp (-x))/2;
	}

	/**Hyperbolic Cosine*/
	public static float Cosh(float x) {
		return (Mathf.Exp (x)+Mathf.Exp (-x))/2;
	}

	/**Hyperbolic Tangent*/
	public static float Tanh(float x) {
		var e2x = Mathf.Exp(2*x);
		return (e2x-1)/(e2x+1);
	}

	/** Simple in and out curve: t*(1-t)*4 */
	public static float T1mT(float t) {
		return t*(1-t)*4;
	}

	/** Simple in and out curve: t*t*(1-t)*(27/4) */
	public static float TT1mT(float t) {
		return t*t*(1-t)*(27/4);
	}

	/** Simple in and out curve: t*(1-t)*(1-t)*(27/4) */
	public static float T1mT1mT(float t) {
		return t*(1-t)*(1-t)*(27/4);
	}

	/** Any given in-out curve of the form t^a * (1-t)^b * c */
	public static ParametricCurve Ta1mTb(float a, float b) {
		var max = 0f;
		if(a+b != 0 && a*b != 0) {
			max = a/(a+b);
		}
		ParametricCurve curve = t=> Mathf.Pow(t,a)*Mathf.Pow(1-t,b);
		var c = 1/curve(max);
		return t=>curve(t)*c;
	}

	// Random functions
	/** Returns a random number between -1 and 1 */
	public static float Random11() {
		return UnityEngine.Random.value*2-1;
	}

	/** Returns the center of a circle defined by 3 points on the edge of the circle.
		Undefined behavior if all three points lie on the same line. */
	public static Vector2 Circle3Points(Vector2 a, Vector2 b, Vector2 c){
		//Reorder points to avoid infinte slopes
		if(a.x == b.x){
			Vector2 temp = b;
			b = c;
			c = temp;
		}
		if(b.x == c.x){
			Vector2 temp = b;
			b = a;
			b = temp;
		}
		//let line 1 be the line AB
		float m1 = (b.y - a.y) / (b.x - a.x);
		//let line 2 be the line BC
		float m2 = (c.y - b.y) / (c.x - b.x);
		Vector2 result = new Vector2();
		result.x = (m1*m2*(a.y - c.y) + m2*(a.x+b.x) - m1*(b.x+c.x)) / (2 * (m2 - m1));
		result.y = -(result.x - (a.x + b.x)/2) / m1 + (a.y+b.y)/2;
		return result;
	}

	/**Checks if a value lies between two bounds*/
	public static bool IsBounded(float value, float low, float high) {
		return value > low && value < high;
	}

	/** Returns a relative place in relation to two other numbers */
	public static float Place(float num, float low, float high) {
		return (num-low)/(high-low);
	}

	// Vector component swizzling
	public static Vector2 yx(this Vector2 v) {
		return new Vector2(v.y, v.x);
	}
	public static Vector2 xx(this Vector2 v) {
		return new Vector2(v.x, v.x);
	}
	public static Vector2 yy(this Vector2 v) {
		return new Vector2(v.y, v.y);
	}

	public static Vector2 xx(this Vector3 v) {
		return new Vector2(v.x, v.x);
	}
	public static Vector2 xy(this Vector3 v) {
		return new Vector2(v.x, v.y);
	}
	public static Vector2 xz(this Vector3 v) {
		return new Vector2(v.x, v.z);
	}
	public static Vector2 yx(this Vector3 v) {
		return new Vector2(v.y, v.x);
	}
	public static Vector2 yy(this Vector3 v) {
		return new Vector2(v.y, v.y);
	}
	public static Vector2 yz(this Vector3 v) {
		return new Vector2(v.y, v.z);
	}
	public static Vector2 zx(this Vector3 v) {
		return new Vector2(v.z, v.x);
	}
	public static Vector2 zy(this Vector3 v) {
		return new Vector2(v.z, v.y);
	}
	public static Vector2 zz(this Vector3 v) {
		return new Vector2(v.z, v.z);
	}

	public static Vector3 xxx(this Vector3 v) {
		return new Vector3(v.x, v.x, v.x);
	}
	public static Vector3 xxy(this Vector3 v) {
		return new Vector3(v.x, v.x, v.y);
	}
	public static Vector3 xxz(this Vector3 v) {
		return new Vector3(v.x, v.x, v.z);
	}
	public static Vector3 xyx(this Vector3 v) {
		return new Vector3(v.x, v.y, v.x);
	}
	public static Vector3 xyy(this Vector3 v) {
		return new Vector3(v.x, v.y, v.y);
	}
	public static Vector3 xzx(this Vector3 v) {
		return new Vector3(v.x, v.z, v.x);
	}
	public static Vector3 xzy(this Vector3 v) {
		return new Vector3(v.x, v.z, v.y);
	}
	public static Vector3 xzz(this Vector3 v) {
		return new Vector3(v.x, v.z, v.z);
	}
	public static Vector3 yxx(this Vector3 v) {
		return new Vector3(v.y, v.x, v.x);
	}
	public static Vector3 yxy(this Vector3 v) {
		return new Vector3(v.y, v.x, v.y);
	}
	public static Vector3 yxz(this Vector3 v) {
		return new Vector3(v.y, v.x, v.z);
	}
	public static Vector3 yyx(this Vector3 v) {
		return new Vector3(v.y, v.y, v.x);
	}
	public static Vector3 yyy(this Vector3 v) {
		return new Vector3(v.y, v.y, v.y);
	}
	public static Vector3 yyz(this Vector3 v) {
		return new Vector3(v.y, v.y, v.z);
	}
	public static Vector3 yzx(this Vector3 v) {
		return new Vector3(v.y, v.z, v.x);
	}
	public static Vector3 yzy(this Vector3 v) {
		return new Vector3(v.y, v.z, v.y);
	}
	public static Vector3 yzz(this Vector3 v) {
		return new Vector3(v.y, v.z, v.z);
	}
	public static Vector3 zxx(this Vector3 v) {
		return new Vector3(v.z, v.x, v.x);
	}
	public static Vector3 zxy(this Vector3 v) {
		return new Vector3(v.z, v.x, v.y);
	}
	public static Vector3 zxz(this Vector3 v) {
		return new Vector3(v.z, v.x, v.z);
	}
	public static Vector3 zyx(this Vector3 v) {
		return new Vector3(v.z, v.y, v.x);
	}
	public static Vector3 zyy(this Vector3 v) {
		return new Vector3(v.z, v.y, v.y);
	}
	public static Vector3 zyz(this Vector3 v) {
		return new Vector3(v.z, v.y, v.z);
	}
	public static Vector3 zzx(this Vector3 v) {
		return new Vector3(v.z, v.z, v.x);
	}
	public static Vector3 zzy(this Vector3 v) {
		return new Vector3(v.z, v.z, v.y);
	}
	public static Vector3 zzz(this Vector3 v) {
		return new Vector3(v.z, v.x, v.x);
	}

	// here, n is any number value
	public static Vector3 xyn(this Vector3 v, float n) {
		return new Vector3(v.x, v.y, n);
	}

}
