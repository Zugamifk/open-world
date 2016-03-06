using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class World : ScriptableObject
    {

        public int width;
        public int height;

        public TileData[] types;

        public Grid grid;

        void OnEnable()
        {
            grid = new Grid();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new Tile();
                    tile.data = types.Random();
                    grid[new Vector3i(x, y, 0)] = tile;
                }
            }
        }
    }
}