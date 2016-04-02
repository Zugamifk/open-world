using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Vector2f16 {

    public float x;
    public float y;

    public Vector2f16(float x, float y)
    {
        this.x = (float)(int)(x * 16) / 16f;
        this.y = (float)(int)(y * 16) / 16f;
    }

    public Vector2f16(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(Vector2f16 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static implicit operator Vector2f16(Vector2 v)
    {
        return new Vector2f16(v.x, v.y);
    }

    public static implicit operator Vector2i(Vector2f16 v)
    {
        return new Vector2i(v.x, v.y);
    }

    public static implicit operator Vector2f16(Vector2i v)
    {
        return new Vector2f16(v.x, v.y);
    }

    public static implicit operator Vector3(Vector2f16 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static implicit operator Vector2f16(Vector3 v)
    {
        return new Vector2f16(v.x, v.y);
    }

    public static Vector2f16 operator +(Vector2f16 a, Vector2f16 b)
    {
        return new Vector2f16(a.x + b.x, a.y + b.y);
    }

    public static Vector2f16 operator -(Vector2f16 a, Vector2f16 b)
    {
        return new Vector2f16(a.x - b.x, a.y - b.y);
    }

    public static Vector2f16 operator *(Vector2f16 a, int i)
    {
        return new Vector2f16(a.x * i, a.y * i);
    }

    public static Vector2f16 operator *(int i, Vector2f16 a)
    {
        return new Vector2f16(i * a.x, i * a.y);
    }

    public static Vector2f16 operator -(Vector2f16 a)
    {
        return Vector2f16.zero - a;
    }

    public override bool Equals(object o)
    {
        return o is Vector2f16 && ((Vector2f16)o) == this;
    }

    public static bool operator ==(Vector2f16 a, Vector2f16 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2f16 a, Vector2f16 b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }

    public static Vector2f16 up = new Vector2f16(0, 1);
    public static Vector2f16 down = new Vector2f16(0, -1);
    public static Vector2f16 right = new Vector2f16(1, 0);
    public static Vector2f16 left = new Vector2f16(-1, 0);
    public static Vector2f16 zero = new Vector2f16(0, 0);
    public static Vector2f16 one = new Vector2f16(1, 1);

}
