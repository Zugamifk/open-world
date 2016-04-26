using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    [System.Serializable]
    public class Tile : Entity
    {
        public const int SurfaceCount = 2;
        // 1 -> colliding tile, 0 -> non colliding tile, starting from top right and going counterclockwise
        public enum Surface
        {
            Null = 0, // unset or disconnected
            // values 1-255 denote actual flags corresponding to bytes
            TopRight = 1 << 0,
            Top = 1<<1, 
            TopLeft = 1<<2,
            Left = 1<<3,
            BottomLeft = 1<<4,
            Bottom = 1<<5,
            BottomRight = 1<<6,
            Right = 1<<7,
            All = 255,
            // from 256, these are special values
            NotAll = 256, // some are free
            Any = 257, // any possible surfaces
            ValueCount = 258
        }

        // for rotating tile sprites
        public enum Rotation
        {
            Down,
            Right,
            Up,
            Left
        }

        public TileData tileData;
        public Vector2i gridPosition;
        public Surface surface;
        [Binary]
        public byte surfaceBits;
        [Tooltip("Distance from the nearest surface, for graphics and maybe other uses")]
        public int altitude;
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

            if (!set)
            {
                if ((byte)(~neighboursBits) == 0)
                {
                    surface = Surface.All;
                }
                else
                {
                    surface = Surface.NotAll;
                }
            }

            surfaceBits = neighboursBits;
        }

        /// <summary>
        /// are the three proximate surfaces to this surface free? (non-colliding)
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public bool Is3NodesFree(Surface surface)
        {
            //TODO: optimize with smarter bit shifting
            var s = (byte)surface;
            var s0 = s << 1 | s >> 7;
            var s1 = s >> 1 | s << 7;
            if ((surfaceBits & (s0 | s | s1)) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetData(TileData data)
        {
            tileData = data;
        }
    }
}