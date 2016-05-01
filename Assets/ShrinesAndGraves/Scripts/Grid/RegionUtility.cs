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
                if (tiles[i] != null && tiles[i].collides && !tiles[i].tileData.isPlatform)
                {
                    tiles[i].SetData(tile);
                }
            }
        }

        /// <summary>
        /// Update tile altitudes
        /// </summary>
        /// <param name="g">grid to update</param>
        /// <param name="rect">rect to sample grid with</param>
        /// <param name="updateRect">rect to compare altitudes to</param>
        public static void UpdateDepths(Grid g, Recti rect, Recti updateRect)
        {
            var tiles = new Tile[rect.width * rect.height];
            int I = g.GetTiles(rect, tiles);
            for (int i = 0; i < I; i++)
            {
                if (tiles[i] != null)
                {
                    if (updateRect.Contains(tiles[i].gridPosition))
                    {
                        tiles[i].altitude = tiles[i].gridPosition.y - updateRect.yMin;
                    }
                    else
                    {
                        var pos = tiles[i].gridPosition;
                        var dis = Vector2i.zero;
                        if(pos.x < updateRect.center.x) {
                            dis.x = Mathf.Max(-(pos.x - updateRect.xMin), 0);
                        }
                        else
                        {
                            dis.x = Mathf.Max(pos.x - updateRect.xMax, 0);
                        }
                        if (pos.y < updateRect.center.y)
                        {
                            dis.y = Mathf.Max(-(pos.y - updateRect.yMin), 0);
                        }
                        else
                        {
                            dis.y = Mathf.Max(pos.y - updateRect.yMax, 0);
                        }
                        tiles[i].altitude = Mathf.Max(-Mathf.Max(dis.x, dis.y), tiles[i].altitude);
                    }
                }
            }
        }
    }
}