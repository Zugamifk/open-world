using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class TileGraphicMetaData
    {
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
    }
}