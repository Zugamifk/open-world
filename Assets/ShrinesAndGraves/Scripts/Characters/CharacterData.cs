using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class CharacterData : WorldObjectData
    {
        public GameObject graphicsPrefab;
        public bool canMove;
        [Tooltip("Set to -1 to make invincible")]
        public int hitpoints;

        public bool canInteract;

        public DialogTree dialog;
    }
}