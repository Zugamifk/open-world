using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class WorldData : ScriptableObject
    {
        public static float SanityBound = 2500;
        public int width;
        public int height;

        public TileData[] types;

        public Vector2 spawnPosition;

        public Environment environment;
    }
}