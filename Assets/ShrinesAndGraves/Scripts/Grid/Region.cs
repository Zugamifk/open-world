using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    public class Region : ScriptableObject
    {
        [System.Serializable]
        public class ObjectSpawn
        {
            public string key;
            public WorldObjectData data;
            public Vector2i spawnPosition;
        }

        protected Dictionary<string, ObjectSpawn> objects;

        public Environment environment;

        public Recti rect;

        public ObjectSpawn[] objectSpawns;

        void OnEnable()
        {
            objects = new Dictionary<string, ObjectSpawn>();
            foreach (var o in objectSpawns)
            {
                objects.Add(o.key, o);
            }
        }

        public virtual void Fill(Grid g)
        {

        }
    }
}