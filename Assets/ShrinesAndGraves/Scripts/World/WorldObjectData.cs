using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObjectData : ScriptableObject
    {

        public Sprite[] sprites;
        public GameObject graphicsPrefab;
        public bool overrideSpriteSize;
        public Vector2f16 overrideSize;
        public bool isTrigger;
        public Grid.Layer layer;

        public virtual Sprite GetSprite()
        {
            return sprites.Random();
        }
    }
}