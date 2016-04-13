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

            int level = g.surface[rect.center.x].gridPosition.y;
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                var surface = g.surface[x];
                var surfHeight = level + (int)(heightScalar * heights.Evaluate((float)(x-rect.xMin) / (float)rect.width));

                for (int y = surface.gridPosition.y; y > surfHeight; y--)
                {
                    g.SetTileData(x, y, environment.tileTypes[1]);
                }

                for (int y = surface.gridPosition.y; y <= surfHeight; y++)
                {
                    g.SetTileData(x, y, environment.tileTypes[0]);
                }
                g.surface[x] = g.GetTile(x, surfHeight);
            }
        }
    }
}