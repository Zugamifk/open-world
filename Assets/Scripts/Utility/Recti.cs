using UnityEngine;
using System.Collections;

namespace Extensions
{
    [System.Serializable]
    public struct Recti 
    {
        public Vector2i position;
        public Vector2i size;

        public Recti(int x, int y, int w, int h) {
            position = new Vector2i(x,y);
            size = new Vector2i(w,h);
        }

        public Recti(Vector2i pos, Vector2i sz) {
            position = pos;
            size = sz;
        }

        public Vector2i center {
            get {
                return position + size/2;
            }
        }

        public int height
        {
            get
            {
                return size.y;
            }
            set
            {
                size.y = value;
            }
        }

        public int width
        {
            get
            {
                return size.x;
            }
            set
            {
                size.x = value;
            }
        }

        public Vector2i max
        {
            get {
                return new Vector2i(
                    Mathf.Max(position.x, position.x + size.x),
                    Mathf.Max(position.y, position.x + size.y)
                );
            }
        }
        
        public Vector2i min
        {
            get
            {
                return new Vector2i(
                    Mathf.Min(position.x, position.x + size.x),
                    Mathf.Min(position.y, position.x + size.y)
                );
            }
        }

        public int x
        {
            get
            {
                return position.x;
            }
            set
            {
                position.x = value;
            }
        }

        public int y
        {
            get
            {
                return position.y;
            }
            set
            {
                position.y = value;
            }
        }

        public int xMax
        {
            get
            {
                return Mathf.Max(position.x, position.x + size.x);
            }
            set
            {
                if (size.x < 0)
                {
                    position.x = value;
                }
                else
                {
                    position.x = value - size.x;
                }
            }
        }

        public int xMin
        {
            get
            {
                return Mathf.Min(position.x, position.x + size.x);
            }
            set
            {
                if (size.y > 0)
                {
                    position.x = value;
                }
                else
                {
                    position.x = value - size.x;
                }
            }
        }

        public int yMax
        {
            get
            {
                return Mathf.Max(position.y, position.y + size.y);
            }
            set
            {
                if (size.y < 0)
                {
                    position.y = value;
                }
                else
                {
                    position.y = value - size.y;
                }
            }
        }

        public int yMin
        {
            get
            {
                return Mathf.Min(position.y, position.y + size.y);
            }
            set
            {
                if (size.y > 0)
                {
                    position.y = value;
                }
                else
                {
                    position.y = value - size.y;
                }
            }
        }

        public bool Contains(Recti other)
        {
            return other.xMin >= xMin &&
                other.xMax <= xMax &&
                other.yMin >= yMin &&
                other.yMax <= yMax;
        }

        public bool Contains(Vector2i point)
        {
            return point.x >= xMin &&
                point.x <= xMax &&
                point.y >= yMin &&
                point.y <= yMax;
        }

        public bool Overlaps(Recti other)
        {
            return other.xMin < xMax &&
                other.xMax > xMin &&
                other.yMin < yMax &&
                other.yMax > yMin;
        }

        public void Set(int x, int y, int w, int h)
        {
            position.x = x;
            position.y = y;
            size.x = w;
            size.y = h;
        }

        public override string ToString()
        {
            return string.Format ("(x:{0} y:{1} w:{2} h:{3})", x, y, size.x, size.y);
        }
    }
}