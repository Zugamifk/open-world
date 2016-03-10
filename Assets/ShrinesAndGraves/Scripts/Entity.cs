using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Entity
    {
        public virtual string name { get { return "Entity"; } }
        public Vector2 position;
        public WorldObject viewObject;
    }
}