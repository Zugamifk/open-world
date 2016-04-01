using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    [System.Serializable]
    public class Tile : Entity
    {
        public const int SurfaceCount = 2;
        public enum Surface
        {
            Null,
            Top,
            None, // inside other tiles
            All = 255
        }

        public TileData tileData;
        public Vector2i gridPosition;
        public Surface surface;
        public byte surfaceBits;
        public List<Entity> contained;

        public static Tile Null
        {
            get
            {
                return new Tile();
            }
        }

        public bool collides
        {
            get
            {
                if (tileData != null)
                {
                    return tileData.collides;
                }
                else
                {
                    return false;
                }
            }
        }

        public Rect rect
        {
            get
            {
                return new Rect(gridPosition, Vector2.one);
            }
        }

        public override string name
        {
            get
            {
                return "Tile " + gridPosition.ToString();
            }
        }

        public void SetSurface(byte neighboursBits)
        {
            if ((neighboursBits & (1<<1)) == 0)
            {
                surface = Surface.Top;
            }
            else if (neighboursBits == 255)
            {
                surface = Surface.None;
            }

            surfaceBits = neighboursBits;
        }
    }
}