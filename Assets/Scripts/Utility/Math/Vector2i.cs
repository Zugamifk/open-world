using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Vector2i {

    public int x;
    public int y;

    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2i(float x, float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    public static implicit operator Vector2(Vector2i v)
    {
        return new Vector2(v.x, v.y);
    }

    public static implicit operator Vector2i(Vector2 v)
    {
        return new Vector2i((int)v.x, (int)v.y);
    }

    public static explicit operator Vector3(Vector2i v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static explicit operator Vector2i(Vector3i v)
    {
        return new Vector2i(v.x, v.y);
    }

    public static explicit operator Vector3i(Vector2i v)
    {
        return new Vector3i(v.x, v.y, 0);
    }

    public static explicit operator Vector2i(Vector3 v)
    {
        return new Vector2i((int)v.x, (int)v.y);
    }

    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x + b.x, a.y + b.y);
    }

    public static Vector2i operator +(Vector2i a, int b)
    {
        return new Vector2i(a.x + b, a.y + b);
    }

    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x - b.x, a.y - b.y);
    }

    public static Vector2i operator -(Vector2i a, int b)
    {
        return new Vector2i(a.x - b, a.y - b);
    }

    public static Vector2i operator *(Vector2i a, int i)
    {
        return new Vector2i(a.x * i, a.y * i);
    }

    public static Vector2i operator *(int i, Vector2i a)
    {
        return new Vector2i(i * a.x, i * a.y);
    }

    public static Vector2i operator /(int i, Vector2i a)
    {
        return new Vector2i(i / a.x, i / a.y);
    }

    public static Vector2i operator /(Vector2i a, int i)
    {
        return new Vector2i(a.x / i, a.y / i);
    }

    public static Vector2i operator %(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x % b.x, a.y % b.y);
    }

    public static Vector2i operator %(Vector2i a, int i)
    {
        return new Vector2i(a.x % i, a.y % i);
    }

    public static Vector2i operator -(Vector2i a)
    {
        return Vector2i.zero - a;
    }

    public override bool Equals(object o)
    {
        return o is Vector2i && ((Vector2i)o) == this;
    }

    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator ==(Vector2i a, int b)
    {
        return a.x == b && a.y == b;
    }

    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !(a == b);
    }

    public static bool operator !=(Vector2i a, int b)
    {
        return !(a == b);
    }


    public int Get30BitZCurveValue()
    {
        var xBias = x + 512;
        var yBias = y + 512;

        var interleaved = 0;

        for (int i = 0; i < 10; i++)
        {
            interleaved |=
                      (xBias & 1 << i) << i * 2
                    | (yBias & 1 << i) << (i * 2 + 1);
        }

        return interleaved;
    }

    public int CompareTo(Vector3i other)
    {
        return Get30BitZCurveValue().CompareTo(other.Get30BitZCurveValue());
    }

    public override int GetHashCode()
    {
        return Get30BitZCurveValue();
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }

    public static Vector2i up = new Vector2i(0, 1);
    public static Vector2i down = new Vector2i(0, -1);
    public static Vector2i right = new Vector2i(1, 0);
    public static Vector2i left = new Vector2i(-1, 0);
    public static Vector2i zero = new Vector2i(0, 0);
    public static Vector2i one = new Vector2i(1, 1);
}
