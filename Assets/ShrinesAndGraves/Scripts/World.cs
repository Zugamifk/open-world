using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class World : ScriptableObject
    {
        public static float SanityBound = 2500;
        public int width;
        public int height;

        public TileData[] types;

        public Grid grid;

        public Vector2 spawnPosition;

        void OnEnable()
        {
            grid = new Grid(width, height);

            grid.width = width;
            grid.height = height;
            
            Generate();
        }

        void Generate()
        {
            var noise = Math.FrequencyNoise1D(x => x, x => x*x, x => (1-x)*(1 - x), 0.1f, 1, 4);
            for (int x = 0; x < width; x++)
            {
                var g = noise(x);
                for (int y = 0; y < height; y++)
                {
                    var tile = new Tile();

                    if (y < 25+g*50)
                    {
                        tile.data = types[0];
                    }
                    else
                    {
                        tile.data = types[1];
                    }
                    tile.gridPosition = new Vector3i(x, y, 0);
                    grid.SetTile(x, y, tile);
                }
            }
        }
    }
}