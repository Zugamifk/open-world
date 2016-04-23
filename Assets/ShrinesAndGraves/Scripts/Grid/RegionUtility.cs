using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public static class RegionUtility
    {

        /// <summary>
        /// fills only tiles that collide
        /// </summary>
        public static void FillGroundTiles(Grid g, Recti rect, TileData tile)
        {
            var tiles = new Tile[rect.width * rect.height];
            int I = g.GetTiles(rect, tiles);
            for (int i = 0; i < I; i++)
            {
                if (tiles[i] != null && tiles[i].collides)
                {
                    tiles[i].SetData(tile);
                }
            }
        }
    }
}