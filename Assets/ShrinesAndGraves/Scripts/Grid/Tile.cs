using UnityEngine;
using System.Collections;

namespace Shrines
{
    [System.Serializable]
    public class Tile : Entity
    {
        public const int SurfaceCount = 2;
        public enum Surface
        {
            None,
            Top
        }

        public TileData data;
        public Vector2i gridPosition;
        public Surface surface;

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
                if (data != null)
                {
                    return data.collides;
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

        public void SetSurface(uint neighboursBits)
        {
            if ((neighboursBits & (1<<1)) == 0)
            {
                surface = Surface.Top;
            }
        }
    }
}