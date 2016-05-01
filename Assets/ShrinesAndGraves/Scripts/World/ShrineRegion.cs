using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class ShrineRegion : Region
    {
        public AnimationCurve heights;
        public float heightScalar;

        public override void Fill(Grid g)
        {

            int level = g.surface[rect.xMin].gridPosition.y;
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                var surface = g.surface[x];
                var surfHeight = level + (int)(heightScalar * heights.Evaluate((float)(x-rect.xMin) / (float)rect.width));

                for (int y = level; y < level + heightScalar; y++)
                {
                    var tile = g.GetTile(x, y);
                    if (y <= surfHeight)
                    {
                        tile.tileData = environment.GetTileData("ground");
                    }
                    else
                    {
                        tile.tileData = environment.GetTileData("empty");
                    }
                    tile.altitude = y - surfHeight;
                }
                g.surface[x] = g.GetTile(x, surfHeight);
            }
        }
    }
}