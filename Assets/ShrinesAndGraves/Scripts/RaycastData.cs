using UnityEngine;
using System.Collections;

namespace Shrines
{
    public struct RaycastData
    {
        public Vector2 point;
        public float distance;
        public Vector2 normal;
        public PhysicsBody other;
        public bool collided;
    }
}