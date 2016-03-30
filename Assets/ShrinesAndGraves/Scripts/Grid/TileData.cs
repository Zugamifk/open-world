using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class TileData : ScriptableObject
    {
        [System.Serializable]
        public struct Graphic
        {
            public Tile.Surface surface;
            public Sprite sprite;
        }

        Dictionary<Tile.Surface, Graphic> graphicLookup = new Dictionary<Tile.Surface,Graphic>();

        [SerializeField]
        public Graphic[] graphics;
        public Sprite defaultSprite;
        public bool collides;

        void OnEnable()
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphicLookup.Add(graphics[i].surface, graphics[i]);
            }
        }

        public Sprite GetSprite(Tile.Surface surface)
        {
            Graphic g;
            if (graphicLookup.TryGetValue(surface, out g))
            {
                return g.sprite;
            }
            else return defaultSprite;
        }
    }
}