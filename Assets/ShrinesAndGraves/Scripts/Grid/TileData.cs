using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class TileData : ScriptableObject
    {
        [System.Serializable]
        public class Graphic
        {
            public Tile.Surface surface;
            public Sprite[] sprites;

            public Sprite sprite
            {
                get
                {
                    return sprites.Random();
                }
            }

            public static bool operator ==(Graphic a, Graphic b) {
                if(object.ReferenceEquals(a, b)) 
                {
                    return true;
                } 
                else if (object.ReferenceEquals(a, null))
                {
                    return b.surface == Tile.Surface.Null;
                } 
                else if (object.ReferenceEquals(b, null))
                {
                    return a.surface == Tile.Surface.Null;
                }
                else
                {
                    return false;
                }
            }

            public static bool operator !=(Graphic a, Graphic b)
            {
                return !(a == b);
            }
        }

        [System.NonSerialized]
        Graphic[] graphicLookup = new Graphic[256];

        [SerializeField]
        public Graphic[] graphics;
        public Sprite defaultSprite;
        public bool collides;

        void OnEnable()
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphicLookup[(byte)graphics[i].surface] =  graphics[i];
            }
        }

        public Sprite GetSprite(Tile tile)
        {
            Graphic g = graphicLookup[tile.surfaceBits];
            if (g!=null)
            {
                return g.sprite;
            }

            g = graphicLookup[(byte)tile.surface];
            if (g != null)
            {
                graphicLookup[tile.surfaceBits] = g;
                return g.sprite;
            }
            
            for (int i = 0; i < 8; i++)
            {
                if ((tile.surfaceBits & 1 << i) == 0)
                {
                    g = graphicLookup[(byte)(1 << i)];
                    if (g != null)
                    {
                        graphicLookup[tile.surfaceBits] = g;
                        return g.sprite;
                    }
                }
            }

            return defaultSprite;
            
        }
    }
}