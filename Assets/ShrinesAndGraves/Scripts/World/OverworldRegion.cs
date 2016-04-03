using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class OverworldRegion : Region
    {
        public override void Fill(Grid grid)
        {
            var width = rect.width;
            var height = rect.height;
            var noise = Math.FrequencyNoise1D(x => x, x => x * x, x => (1 - x) * (1 - x), 0.25f, 0.5f, 2);
            for (int x = 0; x < width; x++)
            {
                var g = noise(x);
                for (int y = 0; y < height; y++)
                {
                    var tile = new Tile();

                    if (y < 25 + g * 10)
                    {
                        tile.tileData = environment.tileTypes[0];
                    }
                    else
                    {
                        tile.tileData = environment.tileTypes[1];
                    }
                    tile.gridPosition = new Vector2i(x, y);
                    tile.position = tile.gridPosition;
                    grid.SetTile(x, y, tile);
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = grid.GetTile(x, y);
                    byte neighboursBits = 0;
                    var t = grid.GetTile(x + 1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x + 1, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x + 1, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);

                    tile.SetSurface(neighboursBits);
                }
            }

            environment.FillGrid(grid);
        }
    }
}