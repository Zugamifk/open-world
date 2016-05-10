using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class TileGraphicData : ScriptableObject
    {

        [System.Serializable]
        public class SpriteShape
        {
            [Tooltip("the AABB for this shape")]
            public Vector2i dimensions = Vector2i.one;
            [Tooltip("offset for the AABB, when deciding if this choice is a valid choice for a tiling")]
            public Vector2i offset;
            [Tooltip ("what increments in the grid this tile can appear on")]
            public Vector2i step;
            public Sprite[] sprites;

            public bool CanPosition(Vector2i pos)
            {
                return (pos - offset) % step == 0;
            }
        }

        [System.Serializable]
        public class SpriteLayer
        {
            public Grid.Layer layer;
            public SpriteShape[] shapes;
            public int defaultShape;

            [System.NonSerialized]
            public List<List<SpriteShape>> sortedShapes = new List<List<SpriteShape>>();

            public void Init()
            {
                for (int s = 0; s < shapes.Length; s++)
                {
                    var shape = shapes[s];
                    List<SpriteShape> list = null;
                    int ss = 0;
                    for (; ss < sortedShapes.Count; ss++)
                    {
                        var sl = sortedShapes[ss];
                        if (sl.Count > 0){
                            if(sl[0].dimensions == shape.dimensions)
                            {
                                list = sl;
                                break;
                            } else if(sl[0].dimensions.x > shape.dimensions.x ||
                                (sl[0].dimensions.x == shape.dimensions.x && sl[0].dimensions.y > shape.dimensions.y)) {
                                break;
                            }
                        }
                        else
                        {
                            Debug.LogError("Empty list found in sorted shapes! This shoul never happen!");
                            sortedShapes.RemoveAt(ss);
                            ss--;
                        }
                    }

                    if (list == null)
                    {
                        list = new List<SpriteShape>();
                        sortedShapes.Insert(ss, list);
                    }

                    list.Add(shape);
                }

                //Debug.Log("*******************************************************");
                //for (int i = 0; i < sortedShapes.Count; i++)
                //{
                //    Debug.Log(sortedShapes[i][0].dimensions);
                //}
            }

            public Vector2i dimensions
            {
                get
                {
                    if (shapes.Length == 0) return Vector2i.one;
                    return shapes[defaultShape].dimensions;
                }
            }

            public Sprite[] sprites
            {
                get
                {
                    if (shapes.Length == 0) return null;
                    return shapes[defaultShape].sprites;
                }
            }
        }

        /// <summary>
        /// For tiles on the 'surface' ie, adjacent to a non-colliding tile
        /// </summary>
        [System.Serializable]
        public class SurfaceGraphic
        {
            public Tile.Surface surface;
            public SpriteLayer[] sprites;

            //public Sprite GetSprite(int layer)
            //{
            //    for (int i = 0; i < sprites.Length; i++)
            //    {
            //        if (sprites[i].layer == layer)
            //        {
            //            return sprites[i].sprites.Random();
            //        }
            //    }
            //    return null;
            //}

            public static bool operator ==(SurfaceGraphic a, SurfaceGraphic b) {
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

            public static bool operator !=(SurfaceGraphic a, SurfaceGraphic b)
            {
                return !(a == b);
            }
        }

        /// <summary>
        /// for tiles surrounded by other tiles
        /// </summary>
        [System.Serializable]
        public class DepthLayer
        {
            public SpriteLayer sprites;
            public int depth;
        }

        SpriteLayer[][] graphicLookup = new SpriteLayer[(int)Grid.Layer.Count][];
        List<DepthLayer>[] depthGraphicLookup = new List<DepthLayer>[(int)Grid.Layer.Count];

        [SerializeField]
        public SurfaceGraphic[] graphics;
        [SerializeField]
        public DepthLayer[] depthlayers;
        public SpriteLayer[] defaultSprites;
        public TileGraphicData defaultTile;

        void OnEnable()
        {
            SpriteLayer[] gs;
            for (int i = 0; i < graphics.Length; i++)
            {
                for(int j=0;j<graphics[i].sprites.Length;j++) {
                    var ss = graphics[i].sprites[j];
                    gs = graphicLookup[(int)ss.layer];
                    if (gs==null)
                    {
                        gs = new SpriteLayer[(int)Tile.Surface.ValueCount];
                        graphicLookup[(int)ss.layer] = gs;


                    }
                    gs[(int)graphics[i].surface] = ss;
                    gs = null;

                    ss.Init();
                }
            }

            List<DepthLayer> ds;
            for (int i = 0; i < depthlayers.Length; i++)
            {
                var ss = depthlayers[i];
                ds = depthGraphicLookup[(int)ss.sprites.layer];
                if (ds==null)
                {
                    ds = new List<DepthLayer>();
                    depthGraphicLookup[(int)ss.sprites.layer] = ds;
                }
                ds.Add(ss);
                ds = null;

                ss.sprites.Init();
            }

            for (int i = 0; i < defaultSprites.Length; i++)
            {
                defaultSprites[i].Init();
            }
        }

        SpriteLayer[] _sprites;
        List<DepthLayer> _depthSprites;
        public Sprite GetSprite(Tile tile, Grid.Layer layer)
        {
            SpriteLayer g = null;
            var all = (byte)(~tile.surfaceBits) == 0;
            if (all)
            {
                _depthSprites = depthGraphicLookup[(int)layer];
                if (_depthSprites!=null && _depthSprites.Count > 0)
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

            _sprites = graphicLookup[(int)layer];
            if (_sprites==null)
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

        __getDefaultGraphic:
            for (int i = 0; i < defaultSprites.Length; i++)
            {
                if (defaultSprites[i].layer == layer)
                {
                    g = defaultSprites[i];
                    goto __testGraphic;
                }
            }

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