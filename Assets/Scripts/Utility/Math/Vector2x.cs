using UnityEngine;
using System.Collections;

namespace Extensions {
	public static class Vector2x {
		public static Vector2 xx(this Vector2 v) {
			return new Vector2(v.x, v.x);
		}

		public static Vector2 xy(this Vector2 v) {
			return new Vector2(v.x, v.y);
		}

		public static Vector2 xn(this Vector2 v, float n) {
			return new Vector2(v.x, n);
		}

		public static Vector2 x0(this Vector2 v) {
			return new Vector2(v.x, 0);
		}

		public static Vector2 x1(this Vector2 v) {
			return new Vector2(v.x, 1);
		}

		public static Vector2 yx(this Vector2 v) {
			return new Vector2(v.y, v.x);
		}

		public static Vector2 yy(this Vector2 v) {
			return new Vector2(v.y, v.y);
		}

		public static Vector2 yn(this Vector2 v, float n) {
			return new Vector2(v.y, n);
		}

		public static Vector2 y0(this Vector2 v) {
			return new Vector2(v.y, 0);
		}

		public static Vector2 y1(this Vector2 v) {
			return new Vector2(v.y, 1);
		}

		public static Vector2 nx(this Vector2 v, float n) {
			return new Vector2(n, v.x);
		}

		public static Vector2 ny(this Vector2 v, float n) {
			return new Vector2(n, v.y);
		}

		public static Vector3 xxx(this Vector2 v) {
			return new Vector3(v.x, v.x, v.x);
		}

		public static Vector3 xxy(this Vector2 v) {
			return new Vector3(v.x, v.x, v.y);
		}

		public static Vector3 xxn(this Vector2 v, float n) {
			return new Vector3(v.x, v.x, n);
		}

		public static Vector3 xx0(this Vector2 v) {
			return new Vector3(v.x, v.x, 0);
		}

		public static Vector3 xx1(this Vector2 v) {
			return new Vector3(v.x, v.x, 1);
		}

		public static Vector3 xyx(this Vector2 v) {
			return new Vector3(v.x, v.y, v.x);
		}

		public static Vector3 xyy(this Vector2 v) {
			return new Vector3(v.x, v.y, v.y);
		}

		public static Vector3 xyn(this Vector2 v, float n) {
			return new Vector3(v.x, v.y, n);
		}

		public static Vector3 xy0(this Vector2 v) {
			return new Vector3(v.x, v.y, 0);
		}

		public static Vector3 xy1(this Vector2 v) {
			return new Vector3(v.x, v.y, 1);
		}

		public static Vector3 xnx(this Vector2 v, float n) {
			return new Vector3(v.x, n, v.x);
		}

		public static Vector3 xny(this Vector2 v, float n) {
			return new Vector3(v.x, n, v.y);
		}

		public static Vector3 xnn(this Vector2 v, float n) {
			return new Vector3(v.x, n, n);
		}

		public static Vector3 xnm(this Vector2 v, float n, float m) {
			return new Vector3(v.x, n, m);
		}

		public static Vector3 xn0(this Vector2 v, float n) {
			return new Vector3(v.x, n, 0);
		}

		public static Vector3 xn1(this Vector2 v, float n) {
			return new Vector3(v.x, n, 1);
		}

		public static Vector3 x0x(this Vector2 v) {
			return new Vector3(v.x, 0, v.x);
		}

		public static Vector3 x0y(this Vector2 v) {
			return new Vector3(v.x, 0, v.y);
		}

		public static Vector3 x0n(this Vector2 v, float n) {
			return new Vector3(v.x, 0, n);
		}

		public static Vector3 x00(this Vector2 v) {
			return new Vector3(v.x, 0, 0);
		}

		public static Vector3 x01(this Vector2 v) {
			return new Vector3(v.x, 0, 1);
		}

		public static Vector3 x1x(this Vector2 v) {
			return new Vector3(v.x, 1, v.x);
		}

		public static Vector3 x1y(this Vector2 v) {
			return new Vector3(v.x, 1, v.y);
		}

		public static Vector3 x1n(this Vector2 v, float n) {
			return new Vector3(v.x, 1, n);
		}

		public static Vector3 x10(this Vector2 v) {
			return new Vector3(v.x, 1, 0);
		}

		public static Vector3 x11(this Vector2 v) {
			return new Vector3(v.x, 1, 1);
		}

		public static Vector3 yxx(this Vector2 v) {
			return new Vector3(v.y, v.x, v.x);
		}

		public static Vector3 yxy(this Vector2 v) {
			return new Vector3(v.y, v.x, v.y);
		}

		public static Vector3 yxn(this Vector2 v, float n) {
			return new Vector3(v.y, v.x, n);
		}

		public static Vector3 yx0(this Vector2 v) {
			return new Vector3(v.y, v.x, 0);
		}

		public static Vector3 yx1(this Vector2 v) {
			return new Vector3(v.y, v.x, 1);
		}

		public static Vector3 yyx(this Vector2 v) {
			return new Vector3(v.y, v.y, v.x);
		}

		public static Vector3 yyy(this Vector2 v) {
			return new Vector3(v.y, v.y, v.y);
		}

		public static Vector3 yyn(this Vector2 v, float n) {
			return new Vector3(v.y, v.y, n);
		}

		public static Vector3 yy0(this Vector2 v) {
			return new Vector3(v.y, v.y, 0);
		}

		public static Vector3 yy1(this Vector2 v) {
			return new Vector3(v.y, v.y, 1);
		}

		public static Vector3 ynx(this Vector2 v, float n) {
			return new Vector3(v.y, n, v.x);
		}

		public static Vector3 yny(this Vector2 v, float n) {
			return new Vector3(v.y, n, v.y);
		}

		public static Vector3 ynn(this Vector2 v, float n) {
			return new Vector3(v.y, n, n);
		}

		public static Vector3 ynm(this Vector2 v, float n, float m) {
			return new Vector3(v.y, n, m);
		}

		public static Vector3 yn0(this Vector2 v, float n) {
			return new Vector3(v.y, n, 0);
		}

		public static Vector3 yn1(this Vector2 v, float n) {
			return new Vector3(v.y, n, 1);
		}

		public static Vector3 y0x(this Vector2 v) {
			return new Vector3(v.y, 0, v.x);
		}

		public static Vector3 y0y(this Vector2 v) {
			return new Vector3(v.y, 0, v.y);
		}

		public static Vector3 y0n(this Vector2 v, float n) {
			return new Vector3(v.y, 0, n);
		}

		public static Vector3 y00(this Vector2 v) {
			return new Vector3(v.y, 0, 0);
		}

		public static Vector3 y01(this Vector2 v) {
			return new Vector3(v.y, 0, 1);
		}

		public static Vector3 y1x(this Vector2 v) {
			return new Vector3(v.y, 1, v.x);
		}

		public static Vector3 y1y(this Vector2 v) {
			return new Vector3(v.y, 1, v.y);
		}

		public static Vector3 y1n(this Vector2 v, float n) {
			return new Vector3(v.y, 1, n);
		}

		public static Vector3 y10(this Vector2 v) {
			return new Vector3(v.y, 1, 0);
		}

		public static Vector3 y11(this Vector2 v) {
			return new Vector3(v.y, 1, 1);
		}

		public static Vector3 nxx(this Vector2 v, float n) {
			return new Vector3(n, v.x, v.x);
		}

		public static Vector3 nxy(this Vector2 v, float n) {
			return new Vector3(n, v.x, v.y);
		}

		public static Vector3 nxn(this Vector2 v, float n) {
			return new Vector3(n, v.x, n);
		}

		public static Vector3 nxm(this Vector2 v, float n, float m) {
			return new Vector3(n, v.x, m);
		}

		public static Vector3 nx0(this Vector2 v, float n) {
			return new Vector3(n, v.x, 0);
		}

		public static Vector3 nx1(this Vector2 v, float n) {
			return new Vector3(n, v.x, 1);
		}

		public static Vector3 nyx(this Vector2 v, float n) {
			return new Vector3(n, v.y, v.x);
		}

		public static Vector3 nyy(this Vector2 v, float n) {
			return new Vector3(n, v.y, v.y);
		}

		public static Vector3 nyn(this Vector2 v, float n) {
			return new Vector3(n, v.y, n);
		}

		public static Vector3 nym(this Vector2 v, float n, float m) {
			return new Vector3(n, v.y, m);
		}

		public static Vector3 ny0(this Vector2 v, float n) {
			return new Vector3(n, v.y, 0);
		}

		public static Vector3 ny1(this Vector2 v, float n) {
			return new Vector3(n, v.y, 1);
		}

		public static Vector3 nnx(this Vector2 v, float n) {
			return new Vector3(n, n, v.x);
		}

		public static Vector3 nmx(this Vector2 v, float n, float m) {
			return new Vector3(n, m, v.x);
		}

		public static Vector3 nny(this Vector2 v, float n) {
			return new Vector3(n, n, v.y);
		}

		public static Vector3 nmy(this Vector2 v, float n, float m) {
			return new Vector3(n, m, v.y);
		}

		public static Vector3 nnn(this Vector2 v, float n) {
			return new Vector3(n, n, n);
		}

		/** this is more useful than you think */
		public static Vector3 nmo(this Vector2 v, float n, float m, float o) {
			return new Vector3(n, m, o);
		}

		public static Vector3 nn0(this Vector2 v, float n) {
			return new Vector3(n, n, 0);
		}

		public static Vector3 nm0(this Vector2 v, float n, float m) {
			return new Vector3(n, m, 0);
		}

		public static Vector3 nn1(this Vector2 v, float n) {
			return new Vector3(n, n, 1);
		}

		public static Vector3 nm1(this Vector2 v, float n, float m) {
			return new Vector3(n, m, 1);
		}

		public static Vector3 n0x(this Vector2 v, float n) {
			return new Vector3(n, 0, v.x);
		}

		public static Vector3 n0y(this Vector2 v, float n) {
			return new Vector3(n, 0, v.y);
		}

		public static Vector3 n0n(this Vector2 v, float n) {
			return new Vector3(n, 0, n);
		}

		public static Vector3 n0m(this Vector2 v, float n, float m) {
			return new Vector3(n, 0, m);
		}

		public static Vector3 n00(this Vector2 v, float n) {
			return new Vector3(n, 0, 0);
		}

		public static Vector3 n01(this Vector2 v, float n) {
			return new Vector3(n, 0, 1);
		}

		public static Vector3 n1x(this Vector2 v, float n) {
			return new Vector3(n, 1, v.x);
		}

		public static Vector3 n1y(this Vector2 v, float n) {
			return new Vector3(n, 1, v.y);
		}

		public static Vector3 n1n(this Vector2 v, float n) {
			return new Vector3(n, 1, n);
		}

		public static Vector3 n1m(this Vector2 v, float n, float m) {
			return new Vector3(n, 1, m);
		}

		public static Vector3 n10(this Vector2 v, float n) {
			return new Vector3(n, 1, 0);
		}

		public static Vector3 n11(this Vector2 v, float n) {
			return new Vector3(n, 1, 1);
		}

	}
}
