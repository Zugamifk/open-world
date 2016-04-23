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
            Null = 0,
            TopRight = 1 << 0,
            Top = 1<<1, 
            TopLeft = 1<<2,
            Left = 1<<3,
            BottomLeft = 1<<4,
            Bottom = 1<<5,
            BottomRight = 1<<6,
            Right = 1<<7,
            None = 1 << 8, // inside other tiles
            All = 255
        }

        public TileData tileData;
        public Vector2i gridPosition;
        public Surface surface;
        [Binary]
        public byte surfaceBits;
        public List<Entity> contained = new List<Entity>();

        public static Tile Null
        {
            get
            {
                return new Tile(0,0,null);
            }
        }

        public Tile(int x, int y, TileData data) : base()
        {
            this.position = new Vector2(x,y);
            tileData = data;
            gridPosition = new Vector2i(x, y);
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

        public override string name
        {
            get
            {
                return "Tile " + gridPosition.ToString();
            }
        }

        // order in which to check surfaces
        static int[] s_surfaceOrder3 = new int[] { 0, 2, 1, 3, 7, 4, 6, 5 };
        static int[] s_surfaceOrder1 = new int[] { 1, 3, 7, 5, 2, 4, 6, 0 };

        // set the "surface" of a tile
        public void SetSurface(byte neighboursBits)
        {
            bool set = false;
            // check surface based on 3 consecutive free tiles first
            for (int i = 0; i < 8; i++)
            {
                //TODO: optimize with smarter bit shifting
                var s = s_surfaceOrder3[i];
                var s0 = Extensions.Math.Mod(s - 1, 8);
                var s1 = (s + 1) % 8;
                if ((neighboursBits & (1 << s0 | 1<<s | 1 << s1)) == 0)
                {
                    surface = (Surface)(1 << s);
                    set = true;
                    break;
                }
            }

            // check for a single free tile if still unset
            if (!set )
            {
                for (int i = 0; i < 8; i++)
                {
                    var s = s_surfaceOrder1[i];
                    if ((neighboursBits & 1 << s) == 0)
                    {
                        surface = (Surface)(1 << s);
                        break;
                    }
                }
            }

            surfaceBits = neighboursBits;
        }

        public void SetData(TileData data)
        {
            tileData = data;
        }
    }
}