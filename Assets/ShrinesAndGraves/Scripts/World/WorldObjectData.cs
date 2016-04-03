using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObjectData : ScriptableObject
    {

        public Sprite[] sprites;

        public Sprite GetSprite()
        {
            return sprites.Random();
        }
    }
}