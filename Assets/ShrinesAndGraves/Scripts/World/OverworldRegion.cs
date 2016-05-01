using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class OverworldRegion : Region
    {
        public int surfaceHeight;
        public int surfaceNoise;

        public override void Fill(Grid grid)
        {
            var width = rect.width;
            var height = rect.height;
            var noise = Math.FrequencyNoise1D(x => x, x => x * x, x => (1 - x) * (1 - x), 0.25f, 0.5f, 2);
            for (int x = 0; x < width; x++)
            {
                int g = (int)(surfaceNoise * noise(x));
                for (int y = 0; y < height; y++)
                {
                    TileData data = null;
                    if (y <= surfaceHeight + g)
                    {
                        data = environment.GetTileData("ground");
                    }
                    else
                    {
                        data = environment.GetTileData("empty");
                    }
                    var tile = new Tile(x,y, data);
                    tile.altitude = y - surfaceHeight - g;
                    grid.SetTile(x, y, tile);
                    if (y == surfaceHeight + g)
                    {
                        grid.surface[x] = tile;
                    }
                }
            }
            environment.FillGrid(grid);
        }
    }
}