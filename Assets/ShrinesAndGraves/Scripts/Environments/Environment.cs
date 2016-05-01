using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class Environment : ScriptableObject
    {
        [System.Serializable]
        public class ObjectDataToken
        {
            public string name;
            public WorldObjectData data;
        }

        [System.Serializable]
        public class TileDataToken
        {
            public string name;
            public TileData data;
        }

        [SerializeField]
        public ObjectDataToken[] m_Objects;
        public TileDataToken[] tileTypes;

        Dictionary<string, WorldObjectData> m_ObjectLookup = new Dictionary<string, WorldObjectData>();
        Dictionary<string, TileData> m_TileLookup = new Dictionary<string, TileData>();

        public WorldObjectData GetWorldObjectData(string name)
        {
            WorldObjectData data = null;
            m_ObjectLookup.TryGetValue(name, out data);
            return data;
        }

        public TileData GetTileData(string name)
        {
            TileData data = null;
            m_TileLookup.TryGetValue(name, out data);
            return data;
        }

        public virtual void FillGrid(Grid grid)
        {

        }

        void OnEnable()
        {
            foreach (var o in m_Objects)
            {
                if (!string.IsNullOrEmpty(o.name))
                {
                    m_ObjectLookup[o.name] = o.data;
                }
            }

            foreach (var t in tileTypes)
            {
                if (!string.IsNullOrEmpty(t.name))
                {
                    m_TileLookup[t.name] = t.data;
                }
            }
        }
    }
}