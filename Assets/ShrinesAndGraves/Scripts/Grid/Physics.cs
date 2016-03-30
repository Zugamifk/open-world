using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

/*TODO:
 * Friction
 * Bump maps
 * Complicated normals
 * Projection-based collision
 */
namespace Shrines
{
    public static class Physics
    {
        public static Grid grid;

        public static HashSet<PhysicsBody> registeredBodies = new HashSet<PhysicsBody>();

        public static Vector2 globalForces = Vector2.down * 9.8f;

        static Tile[] tileBatch = new Tile[1024];

        public static float Bound(float val)
        {
            return Mathf.Clamp(val, -World.SanityBound, World.SanityBound);
        }

        public static void Update()
        {
            if (grid == null) return;
            RaycastData info;
            var bounceVal = 0.2f;
            foreach (var b in registeredBodies)
            {
                var step = b.velocity * Time.fixedDeltaTime;
                if (step.magnitude > 0)
                {
                    var op = b.position;
                    int maxIters = 10;
                    while (RayCast(b, step, out info) && --maxIters > 0)
                    {
                        var newStep = Vector2.Reflect(step, info.normal);
                        var bounceMag = Vector2.Dot(newStep.normalized, info.normal);
                        var frictionMag = 1 - bounceMag;
                        newStep *= (bounceMag * bounceVal + frictionMag);
                        b.position = info.point + newStep * (step.magnitude - info.distance);
                        var refv = Vector2.Reflect(b.velocity, info.normal);
                        var bv = bounceMag * refv;
                        var fv = refv - bv;
                        b.velocity = fv + bv * bounceVal;
                        step = newStep;

                        b.OnCollision(new Collision(info));
                        //Debug.Log(step.x + " : "+step.y + " : " + info.normal);
                    }
                    //if (maxIters == 0) Debug.Break();
                    b.position += step;
                }
                b.velocity += (b.acceleration+globalForces) * Time.fixedDeltaTime;
            }

        }

        //TODO: use projection and search for intervals with interval tree ??
        static bool RayCast(PhysicsBody b, Vector2 step, out RaycastData info)
        {
            RaycastData hit;
            info = new RaycastData();
            info.distance = float.PositiveInfinity;
            
            var r = b.rect;
            r.position += step;
            var G = grid.GetTiles(r, tileBatch);
            Vector2 sep = Vector2.zero;
            for (int i = 0; i < G; i++)
            {
                var tile = tileBatch[i];
                if (grid.Collides(tile))
                {
                    sep += SeparateAxes(r, tile.rect);
                    info.collided = true;
                }
            }

            if (info.collided)
            {
                var dist = (step + sep/(float)G).magnitude;
                info.point = b.position;
                info.normal = sep.normalized;
                info.distance = dist;
                return true;
            }

            int collisionCorner = -1;
            for (int i = 0; i < 4; i++)
            {
                // redo this "corners" business

                var tile = grid.GetTile(b.position + b.corners[i]*(1-Mathf.Epsilon) + step);
                var collides = grid.Collides(tile);
                //if (collides)
                //{
                //    var r = b.rect;
                //    r.position += step;
                //    var sep = SeparateAxes(r, tile.rect);
                //    var dist = (step + sep).magnitude;
                //    info.point = b.position;
                //    info.normal = sep.normalized;
                //    info.distance = dist;
                //    info.collided = true;
                //    return true;
                //} else
                if (grid.Raycast(b.position + b.corners[i], b.position + b.corners[i] + step, out hit))
                {
                    if (collisionCorner < 0 || hit.distance < info.distance)
                    {
                        info = hit;
                        info.point -= b.corners[i];
                        collisionCorner = i;
                    }
                }
            }
            if (collisionCorner < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static Vector2 SeparateAxes(Rect a, Rect b)
        {
            var x = Mathf.Min(a.xMax - b.xMin, b.xMax - a.xMin) + Mathf.Epsilon;
            var y = Mathf.Min(a.yMax - b.yMin, b.yMax - a.yMin) + Mathf.Epsilon;
            var signs = a.center - b.center;
            if (y > x)
            {
                return Vector2.right * x * Mathf.Sign(signs.x);
            }
            else
            {
                return Vector2.up * y * Mathf.Sign(signs.y);
            }
        }
    }
}