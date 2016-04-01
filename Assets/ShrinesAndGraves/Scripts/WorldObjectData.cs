using UnityEngine;
using System.Collections;

public class WorldObjectData : ScriptableObject {

    public Sprite[] sprites;

    public Sprite GetSprite()
    {
        return sprites.Random();
    }
}
