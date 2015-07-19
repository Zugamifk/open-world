using UnityEngine;
using System.Collections;

namespace Extensions {
	public static class Vector3x {

		public static Vector2 xx(this Vector3 v) {
			return new Vector2(v.x, v.x);
		}
		public static Vector2 xy(this Vector3 v) {
			return new Vector2(v.x, v.y);
		}
		public static Vector2 xz(this Vector3 v) {
			return new Vector2(v.x, v.z);
		}
		public static Vector2 xn(this Vector3 v, float n) {
			return new Vector2(v.x, n);
		}
		public static Vector2 x0(this Vector3 v) {
			return new Vector2(v.x, 0);
		}
		public static Vector2 x1(this Vector3 v) {
			return new Vector2(v.x, 1);
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
		public static Vector2 yn(this Vector3 v, float n) {
			return new Vector2(v.y, n);
		}
		public static Vector2 y0(this Vector3 v) {
			return new Vector2(v.y, 0);
		}
		public static Vector2 y1(this Vector3 v) {
			return new Vector2(v.y, 1);
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
		public static Vector2 zn(this Vector3 v, float n) {
			return new Vector2(v.z, n);
		}
		public static Vector2 z0(this Vector3 v) {
			return new Vector2(v.z, 0);
		}
		public static Vector2 z1(this Vector3 v) {
			return new Vector2(v.z, 1);
		}
		public static Vector2 nx(this Vector3 v, float n) {
			return new Vector2(n, v.x);
		}
		public static Vector2 ny(this Vector3 v, float n) {
			return new Vector2(n, v.y);
		}
		public static Vector2 nz(this Vector3 v, float n) {
			return new Vector2(n, v.z);
		}
		public static Vector2 nn(this Vector3 v, float n) {
			return new Vector2(n, n);
		}
		public static Vector2 nm(this Vector3 v, float n, float m) {
			return new Vector2(n, m);
		}
		public static Vector2 n0(this Vector3 v, float n) {
			return new Vector2(n, 0);
		}
		public static Vector2 n1(this Vector3 v, float n) {
			return new Vector2(n, 1);
		}
	}
}
