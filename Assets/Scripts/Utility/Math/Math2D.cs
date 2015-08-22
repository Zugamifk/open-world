using UnityEngine;
using System.Collections;

namespace Extensions {
    public static class Math2D {
        public static float Cross(this Vector2 first, Vector2 second) {
            return first.x*second.y - first.y*second.x;
        }
        public static bool LineLineIntersection(Vector2 p0, Vector2 p1, Vector2 q0, Vector2 q1) {
            var P = p1-p0;
            var Q = q1-q0;

            var PxQ = P.Cross(Q);
            var P2Q = (q0-p0).Cross(P);

            if(Mathf.Approximately(PxQ, 0)) {
                if(Mathf.Approximately(P2Q, 0)) {
                    var PdotQ = Vector2.Dot(P,Q);
                    var t0 = Vector2.Dot(q0-p0, P)/P.sqrMagnitude;
                    var t1 = t0 + PdotQ/P.sqrMagnitude;
                    return Math.Between(t1-t0,0,Mathf.Sign(PdotQ));
                } else {
                    return false;
                }
            } else {
                var t = (q0-p0).Cross(Q)/PxQ;
                var u = P2Q/Q.Cross(P);
                return Math.Between(t,0,1) && Math.Between(u,0,1);
            }
        }

        public static bool LeftOf(Vector2 p0, Vector2 p1, Vector2 q) {
            var area = TriangleArea(p0,p1,q);
            return area > 0 && !Mathf.Approximately(area, 0);
        }

        public static bool LeftOfOrOn(Vector2 p0, Vector2 p1, Vector2 q) {
            var area = TriangleArea(p0,p1,q);
            return area > 0 || Mathf.Approximately(area, 0);
        }

        public static bool Colinear(Vector2 p0, Vector2 p1, Vector2 q) {
            var area = TriangleArea(p0,p1,q);
            return Mathf.Approximately(area, 0);
        }

        public static float TriangleArea(Vector2 a, Vector2 b, Vector2 c) {
            return 0.5f*((b.x-a.x)*(c.y-a.y)-(c.x-a.x)*(b.y-a.y));
        }

        public static bool InCone(Vector2 a, Vector2 b, Vector2 aPrev, Vector2 aSucc) {
            if(LeftOfOrOn(a,aSucc,aPrev)) {
                return LeftOf(a,b,aPrev) && LeftOf(b,a,aSucc);
             } else {
                return !(LeftOfOrOn(a,b,aSucc) && LeftOfOrOn(b,a,aPrev));
            }
        }

        public static float PolygonArea(Vector2 point, params Vector2[] polygonPoints) {
            int N = polygonPoints.Length;
            float area = 0;
            for(int i=0;i<N;i++) {
                area += TriangleArea(point, polygonPoints[i], polygonPoints[(i+1)%N]);
            }
            return area;
        }

        public static Vector2 Rotate(this Vector2 v, float angle) {
            return Quaternion.AngleAxis(angle, Vector3.forward)*v;
        }

    }
}
