using UnityEngine;
using System.Collections;

namespace Shrines
{
    public struct Collision
    {
        public RaycastData data;
        public Collision(RaycastData data)
        {
            this.data = data;
        }
    }
}