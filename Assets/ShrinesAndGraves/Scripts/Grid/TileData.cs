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

        public Sprite GetSprite(Tile tile, int layer)
        {
            if (tile.graphicMetadata.dontDraw) return null;

            return graphics.GetSprite(tile, layer);
        }
    }
}