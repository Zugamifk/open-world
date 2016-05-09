using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObjectData : ScriptableObject
    {

        public Sprite[] sprites;
        public bool isTrigger;
        public Grid.Layer layer;

        public virtual Sprite GetSprite()
        {
            return sprites.Random();
        }
    }
}