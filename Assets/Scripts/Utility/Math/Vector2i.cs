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

    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x + b.x, a.y + b.y);
    }

    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.x - b.x, a.y - b.y);
    }

    public static Vector2i operator *(Vector2i a, int i)
    {
        return new Vector2i(a.x * i, a.y * i);
    }

    public static Vector2i operator *(int i, Vector2i a)
    {
        return new Vector2i(i * a.x, i * a.y);
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

    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !(a == b);
    }

    public static Vector2i up = new Vector2i(0, 1);
    public static Vector2i down = new Vector2i(0, -1);
    public static Vector2i right = new Vector2i(1, 0);
    public static Vector2i left = new Vector2i(-1, 0);
    public static Vector2i zero = new Vector2i(0, 0);
    public static Vector2i one = new Vector2i(1, 1);
}
