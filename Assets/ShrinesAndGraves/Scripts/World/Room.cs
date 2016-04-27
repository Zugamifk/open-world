using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    public class Room
    {
        public enum Side {
            LEFT,
            RIGHT,
            TOP,
            BOTTOM
        }

        public class Exit
        {
            public Side side;
            public Vector2i interval; 
        }

        public Vector2i position;
        public Vector2i size;
        public List<Exit> exits = new List<Exit>();

        public Recti rect
        {
            get
            {
                return new Recti(position, size);
            }
        }

        public Room() { }

        public Room(Recti r)
        {
            position = r.position;
            size = r.size;
        }

        public Room(Vector2i pos, Vector2i sz)
        {
            position = pos;
            size = sz;
        }

        public void AddRandomExits(int num, int exitSize = 4)
        {
            bool t, b, l, r;
            t = b = l = r = false;
            for (int i = 0; i < num; i++)
            {
                if(Randomx.Bool) {
                    if ((Randomx.Bool || b) && !t)
                    {
                        AddRandomExit(Side.TOP, exitSize);
                        t = true;
                    }
                    else if (!b)
                    {
                        AddRandomExit(Side.BOTTOM, exitSize);
                        b = true;
                    }
                }
                else
                {
                    if ((Randomx.Bool || l) && !r)
                    {
                        AddRandomExit(Side.RIGHT, exitSize);
                        r = true;
                    }
                    else if (!l)
                    {
                        AddRandomExit(Side.LEFT, exitSize);
                        l = true;
                    }
                }
            }
        }

        public Exit AddRandomExit(Side s, int len)
        {
            if (s == Side.LEFT || s == Side.RIGHT)
            {
                if (len > size.y) return null;

                var b = Random.Range(0, size.y - len);
                var e = new Exit() { side = s, interval = new Vector2i(b, len) };
                exits.Add(e);
                return e;
            }
            else
            {
                if (len > size.x) return null;

                var b = Random.Range(0, size.x - len);
                var e = new Exit() { side = s, interval = new Vector2i(b, len) };
                exits.Add(e);
                return e;
            }
        }

        public Exit GetExit(Side s)
        {
            for (int i = 0; i < exits.Count; i++)
            {
                if (exits[i].side == s)
                {
                    return exits[i];
                }
            }
            return null;
        }

        public static Side GetOppositeSide(Side s)
        {
            switch (s)
            {
                case Side.LEFT:
                    return Side.RIGHT;
                case Side.RIGHT:
                    return Side.LEFT;
                case Side.TOP:
                    return Side.BOTTOM;
                case Side.BOTTOM:
                    return Side.TOP;
                default:
                    break;
            }
            return Side.TOP;
        }
    }
}