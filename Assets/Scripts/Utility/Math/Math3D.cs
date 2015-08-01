using UnityEngine;
using System.Collections;

namespace Extensions {
    public static class Math3D {
        public static bool LineLineIntersection(Vector3 p0, Vector3 p1, Vector3 q0, Vector3 q1) {
            var P = p1-p0;
            var Q = q1-q0;

            var PxQ = Vector3.Cross(P,Q).magnitude;
            var P2Q = Vector3.Cross(q0-p0, P).magnitude;

            if(Mathf.Approximately(PxQ, 0)) {
                if(Mathf.Approximately(P2Q, 0)) {
                    var PdotQ = Vector3.Dot(P,Q);
                    var t0 = Vector3.Dot(q0-p0, P)/P.sqrMagnitude;
                    var t1 = t0 + PdotQ/P.sqrMagnitude;
                    return Math.Between(t1-t0,0,Mathf.Sign(PdotQ));
                } else {
                    return false;
                }
            } else {
                var t = Vector3.Cross(q0-p0, Q).magnitude/PxQ;
                var u = P2Q/Vector3.Cross(Q,P).magnitude;
                return Math.Between(t,0,1) && Math.Between(u,0,1);
            }
        }

        public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c) {
            return Vector3.Cross(b-a, c-a).magnitude*0.5f;
        }
    }
}
