using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObjectData : ScriptableObject
    {

        public Sprite[] sprites;
        public bool isTrigger;
        [Layer]
        public int sortinglayer;

        public virtual Sprite GetSprite()
        {
            return sprites.Random();
        }
    }
}