using UnityEngine;
using System.Collections;

[System.Serializable]
public class Gradient2D {

    [System.Serializable]
    public struct ColorKey {
        public Color key;
        public Vector2 position;
    }

    [SerializeField]
    public ColorKey[] colorKeys;
    [SerializeField]
    public ColorKey[] alphaKeys;

    public Color Evaluate(Vector2 position)
    {
        return Color.white;
    }
}
