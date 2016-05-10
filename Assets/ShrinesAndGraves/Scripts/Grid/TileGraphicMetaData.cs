using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class TileGraphicMetaData
    {
        [System.Serializable]
        public class GraphicCache
        {
            /// <summary>
            /// check if this tile has been set with a graphic yet
            /// </summary>
            public bool hasOverride
            {
                get
                {
                    return dontDraw || graphicOverride != null;
                }
            }

            /// <summary>
            /// don't give this tile a sprite
            /// </summary>
            public bool dontDraw;

            /// <summary>
            /// size of the graphic that represents this tile
            /// </summary>
            public Vector2i tileGraphicSize;

            /// <summary>
            /// the tile containing the graphic for this tile
            /// </summary>
            public Tile tileWithgraphic;

            /// <summary>
            /// graphic to draw instead of a random one
            /// </summary>
            public Sprite graphicOverride;
        }

        public Dictionary<Grid.Layer, GraphicCache> graphicCache = new Dictionary<Grid.Layer, GraphicCache>();

        public TileGraphicMetaData()
        {
            for(int i=0;i<(int)Grid.Layer.Count;i++) {
                graphicCache.Add((Grid.Layer)i, new GraphicCache());
            }
        }

    }
}