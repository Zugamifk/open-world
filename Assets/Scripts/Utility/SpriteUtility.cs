using UnityEngine;
using System.Collections;

public static class SpriteUtility {

    public static Vector2 Size(this Sprite s)
    {
        return s.rect.size / s.pixelsPerUnit;
    }
}
