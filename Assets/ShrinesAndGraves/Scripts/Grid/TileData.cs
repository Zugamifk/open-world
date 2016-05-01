using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class TileData : ScriptableObject
    {
        [System.Serializable]
        public class SpriteLayer
        {
            [Layer]
            public int layer;
            public Vector2i dimensions = Vector2i.one;
            public Sprite[] sprites;
        }

        [System.Serializable]
        public class Graphic
        {
            public Tile.Surface surface;
            public SpriteLayer[] sprites;

            public Sprite GetSprite(int layer)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (sprites[i].layer == layer)
                    {
                        return sprites[i].sprites.Random();
                    }
                }
                return null;
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

        [System.Serializable]
        public class DepthLayer
        {
            public SpriteLayer sprites;
            public int depth;
        }

        Dictionary<int, SpriteLayer[]> graphicLookup = new Dictionary<int, SpriteLayer[]>();
        Dictionary<int, List<DepthLayer>> depthGraphicLookup = new Dictionary<int, List<DepthLayer>>();

        [SerializeField]
        public Graphic[] graphics;
        [SerializeField]
        public DepthLayer[] depthlayers;
        public SpriteLayer[] defaultSprites;
        public TileData defaultTile;
        public bool collides;
        [Tooltip("Use one-way collisions based on up direction?")]
        public bool isPlatform;

        void OnEnable()
        {
            SpriteLayer[] gs;
            for (int i = 0; i < graphics.Length; i++)
            {
                for(int j=0;j<graphics[i].sprites.Length;j++) {
                    var ss = graphics[i].sprites[j];
                    if (!graphicLookup.TryGetValue(ss.layer, out gs))
                    {
                        gs = new SpriteLayer[(int)Tile.Surface.ValueCount];
                        graphicLookup.Add(ss.layer, gs);
                    }
                    gs[(int)graphics[i].surface] = ss;
                }
            }

            List<DepthLayer> ds;
            for (int i = 0; i < depthlayers.Length; i++)
            {
                var ss = depthlayers[i];
                if (!depthGraphicLookup.TryGetValue(ss.sprites.layer, out ds))
                {
                    ds = new List<DepthLayer>();
                    depthGraphicLookup.Add(ss.sprites.layer, ds);
                }
                ds.Add(ss);
            }
        }

        SpriteLayer[] _sprites;
        List<DepthLayer> _depthSprites;
        public Sprite GetSprite(Tile tile, int layer)
        {
            SpriteLayer g = null;
            var all = (byte)(~tile.surfaceBits) == 0;
            if (all)
            {
                if (depthGraphicLookup.TryGetValue(layer, out _depthSprites))
                {
                    for (int i = 0; i < _depthSprites.Count; i++)
                    {
                        if (-_depthSprites[i].depth >= tile.altitude)
                        {
                            g = _depthSprites[i].sprites;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (g != null)
                    {
                        goto __testGraphic;
                    }
                }
            }
            if (!graphicLookup.TryGetValue(layer, out _sprites))
            {
                goto __getDefaultGraphic;
            }

            g = _sprites[tile.surfaceBits];
            if (g!=null)
            {
                goto __testGraphic;
            }

            g = _sprites[(int)tile.surface];
            if (g != null)
            {
                _sprites[tile.surfaceBits] = g;
                goto __testGraphic;
            }

            if (!all)
            {
                g = _sprites[(int)Tile.Surface.NotAll];
                if (g != null)
                {
                    _sprites[tile.surfaceBits] = g;
                    goto __testGraphic;
                }
            }

            g = _sprites[(int)Tile.Surface.Any];
            if (g != null)
            {
                _sprites[tile.surfaceBits] = g;
                goto __testGraphic;
            }

            for (int i = 0; i < 8; i++)
            {
                if ((tile.surfaceBits & 1 << i) == 0)
                {
                    g = _sprites[(int)(1 << i)];
                    if (g != null)
                    {
                        _sprites[tile.surfaceBits] = g;
                        goto __testGraphic;
                    }
                }
            }

            for (int i = 0; i < defaultSprites.Length; i++)
            {
                if (defaultSprites[i].layer == layer)
                {
                    g = defaultSprites[i];
                    goto __testGraphic;
                }
            }

        __getDefaultGraphic:
            if (defaultTile != null)
            {
                return defaultTile.GetSprite(tile, layer);
            }
            return null;
        __testGraphic:
            if (g.dimensions != 1 && tile.gridPosition % g.dimensions != 0)
            {
                return null;
            }
            else
            {
                return g.sprites.Random();
            }
        }
    }
}