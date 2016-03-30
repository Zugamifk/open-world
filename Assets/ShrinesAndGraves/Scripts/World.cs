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

        public void Initialize(Grid g)
        {
            grid = g;
            width = g.width;
            height = g.height;
        }
    }
}