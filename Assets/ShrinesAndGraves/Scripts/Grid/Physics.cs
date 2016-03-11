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

        public static Vector2 globalForces;

        public static float Bound(float val)
        {
            return Mathf.Clamp(val, -World.SanityBound, World.SanityBound);
        }

        public static void Update()
        {
            if (grid == null) return;
            RaycastData info;
            foreach (var b in registeredBodies)
            {
                var step = b.velocity * Time.fixedDeltaTime;
                var op = b.position;
                if (CheckExtremes(b, step, out info))
                {
                    //Debug.Log(info.point + " : " + b.position + " : " + step + " : " + Vector2.Reflect(step, info.normal)+" : "+info.normal);
                    var newvelocity = Vector2.Reflect(step, info.normal);
                    b.position = info.point + newvelocity*(step.magnitude-info.distance);
                    b.velocity = newvelocity;
                }
                else
                {
                    b.position += step;
                }

                b.velocity += b.acceleration * Time.fixedDeltaTime;
                b.acceleration += globalForces * Time.fixedDeltaTime;
            }
        }

        //TODO: use projection and search for intervals with interval tree ??
        static bool CheckExtremes(PhysicsBody b, Vector2 step, out RaycastData info)
        {
            //if (grid.GetTile(b.position).collides) { Debug.LogWarning("!!!!"); Debug.Break(); }
            RaycastData hit;
            info = new RaycastData();
            int collisionCorner = -1;
            for (int i = 0; i < 4; i++)
            {
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

        
    }
}