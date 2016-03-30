using UnityEngine;
using System.Collections;

namespace Shrines
{
    [System.Serializable]
    public class Entity
    {
        public virtual string name { get { return "Entity"; } }
        public Vector2f16 position;

        [System.NonSerialized]
        public WorldObject viewObject;
    }
}