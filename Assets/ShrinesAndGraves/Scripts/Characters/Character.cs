using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Character : Entity
    {
        public bool canInteract;
        public CharacterData characterData;

        public Character()
            : base()
        {
            canInteract = false;
        }

        public Character(CharacterData cd, Vector2f16 pos) : base(cd, pos)
        {
            canInteract = cd.canInteract;
            characterData = cd;
        }

        public override void OnTriggerEnter(WorldObject wo)
        {
            if (canInteract)
            {
                var po = wo as PlayerObject;
                if (po != null)
                {
                    StartConversation(po);
                }
            }
        }

        public virtual void StartConversation(PlayerObject po)
        {

        }
    }
}