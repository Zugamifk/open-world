using UnityEngine;
using System.Collections;

namespace Shrines
{
    [System.Serializable]
    public class Entity
    {
        public virtual string name { get { return "Entity"; } }
        public Vector2f16 position;
        public WorldObjectData data;
        public Rect rect;

        [System.NonSerialized]
        public WorldObject viewObject;
    }
}