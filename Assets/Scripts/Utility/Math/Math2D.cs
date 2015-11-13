using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public static IEnumerable<Vector2> SuperCover(Vector2 start, Vector2 end) {
            float
                dx = Mathf.Abs(end.x - start.x),
                dy = Mathf.Abs(end.y - start.y),
                error;

            int x = Mathf.FloorToInt(start.x),
                y = Mathf.FloorToInt(start.y),
                n = 1,
                xstep = 0, ystep=0;

            if(Mathf.Approximately(dx, 0)) {
                xstep = 0;
                error = Mathf.Infinity;
            } else if (end.x > start.x) {
                xstep = 1;
                n += Mathf.FloorToInt(end.x) - x;
                error = (Mathf.Floor(start.x)+1 - start.x)*dy;
            } else {
                xstep = -1;
                n += x-Mathf.FloorToInt(end.x);
                error = (start.x-Mathf.Floor(start.x))*dy;
            }

            if(Mathf.Approximately(dy, 0)) {
                ystep = 0;
                error -= Mathf.Infinity;
            } else if (end.y > start.y) {
                ystep = 1;
                n += Mathf.FloorToInt(end.y) - y;
                error -= (Mathf.Floor(start.y)+1 - start.y)*dx;
            } else {
                ystep = -1;
                n += y-Mathf.FloorToInt(end.y);
                error -= (start.y-Mathf.Floor(start.y))*dx;
            }

            for(; n>0;n--) {
                yield return new Vector2(x, y);
                if(error>0) {
                    y += ystep;
                    error-=dx;
                } else {
                    x += xstep;
                    error += dy;
                }
            }

        }

        public static IEnumerable<Vector3i> SuperCover(Vector3i start, Vector3i end) {

            int xstep, ystep;
            int error, errorprev;
            int x = (int)start.x,
                y = (int)start.y;
            int ddx, ddy;
            int dx = (int)end.x-x;
            int dy = (int)end.y-y;

            yield return new Vector3i(x, y, 0);

            if(dx<0) {
                xstep = -1;
                dx = -dx;
            } else {
                xstep = 1;
            }

            if(dy<0) {
                ystep = -1;
                dy = -dy;
            } else {
                ystep = 1;
            }

            ddx = dx * 2;
            ddy = dy * 2;

            if(ddx>=ddy) {
                errorprev = error = dx;
                for(int i=0;i<dx;i++) {
                    x += xstep;
                    error += ddy;
                    if(error>ddx) {
                        y += ystep;
                        error -= ddx;
                        if(error + errorprev < ddx) {
                            yield return new Vector3i(x, y - ystep, 0);
                        } else if (error + errorprev > ddx) {
                            yield return new Vector3i(x - xstep, y, 0);
                        } else {
                            yield return new Vector3i(x, y - ystep, 0);
                            yield return new Vector3i(x - xstep, y, 0);
                        }
                    }
                    yield return new Vector3i(x, y,0);
                    errorprev = error;
                }
            } else {
                errorprev = error = dy;
                for(int i=0;i<dy;i++) {
                    y += ystep;
                    error += ddx;
                    if(error>ddy) {
                        x += xstep;
                        error -= ddy;
                        if(error + errorprev < ddy) {
                            yield return new Vector3i(x - xstep, y, 0);
                        } else if (error + errorprev > ddy) {
                            yield return new Vector3i(x, y - ystep, 0);
                        } else {
                            yield return new Vector3i(x - xstep, y, 0);
                            yield return new Vector3i(x, y - ystep, 0);
                        }
                    }
                    yield return new Vector3i(x, y, 0);
                    errorprev = error;
                }
            }
        }

        public static IEnumerable<Vector3i> CirclePoints(Vector3i origin, int radius) {
            int x, y, decisionOver2;
            for (int octant = 0; octant < 8; octant++)
            {
                x = radius;
                y = 0;
                decisionOver2 = 1 - x;

                while (y <= x)
                {
                    switch (octant)
                    {
                        case 0: yield return new Vector3i(x + origin.x, y + origin.y, 0); break;
                        case 1: yield return new Vector3i(y + origin.x, x + origin.y, 0); break;
                        case 2: yield return new Vector3i(-x + origin.x, y + origin.y, 0); break;
                        case 3: yield return new Vector3i(-y + origin.x, x + origin.y, 0); break;
                        case 4: yield return new Vector3i(-x + origin.x, -y + origin.y, 0); break;
                        case 5: yield return new Vector3i(-y + origin.x, -x + origin.y, 0); break;
                        case 6: yield return new Vector3i(x + origin.x, -y + origin.y, 0); break;
                        case 7: yield return new Vector3i(y + origin.x, -x + origin.y, 0); break;
                    }
                    y++;
                    if (decisionOver2 <= 0)
                    {
                        decisionOver2 += 2 * y + 1;
                    }
                    else
                    {
                        x--;
                        decisionOver2 += 2 * (y - x) + 1;
                    }
                }
            }
        }

    }
}
