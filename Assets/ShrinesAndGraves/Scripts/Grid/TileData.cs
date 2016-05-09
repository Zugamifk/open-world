using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class TileData : ScriptableObject
    {
        public TileGraphicData graphics;
        public bool collides;
        [Tooltip("Use one-way collisions based on up direction?")]
        public bool isPlatform;

        public Sprite GetSprite(Tile tile, Grid.Layer layer)
        {
            var gc = tile.graphicMetadata.graphicCache[layer];
            if (gc.dontDraw) return null;

            if (gc.graphicOverride != null) return gc.graphicOverride;

            return graphics.GetSprite(tile, layer);
        }
    }
}