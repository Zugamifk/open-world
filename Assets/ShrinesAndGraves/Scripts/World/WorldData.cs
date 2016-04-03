using UnityEngine;
using System.Collections;
using Extensions;

namespace Shrines
{
    public class WorldData : ScriptableObject
    {
        public static float SanityBound = 2500;

        public Vector2 spawnPosition;

        public Region emptyRegion;

        public Region[] regions;
    }
}