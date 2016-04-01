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

        //public static HashSet<PhysicsBody> registeredBodies = new HashSet<PhysicsBody>();

        public static Vector2 globalForces = Vector2.down * 9.8f;

        static Tile[] tileBatch = new Tile[1024];

        public static float Bound(float val)
        {
            return Mathf.Clamp(val, -WorldData.SanityBound, WorldData.SanityBound);
        }
    }
}