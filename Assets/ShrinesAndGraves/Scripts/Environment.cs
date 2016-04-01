using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Environment : ScriptableObject {
    [System.Serializable]
    public struct ObjectDataToken
    {
        public string name;
        public WorldObjectData data;
    }

    [SerializeField]
    public ObjectDataToken[] m_Objects;

    Dictionary<string, WorldObjectData> m_ObjectLookup = new Dictionary<string, WorldObjectData>();

    public WorldObjectData GetWorldObjetData(string name)
    {
        WorldObjectData data = null;
        m_ObjectLookup.TryGetValue(name, out data);
        return data;
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
    }
}
