using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Character : Entity
    {
        public bool canInteract;
        public CharacterData characterData;

        bool talking = false;

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

        public override WorldObject GetObject()
        {
            var co =  ObjectManager.Instance.GetPooledComponent<CharacterObject>();
            var cc= ObjectManager.Instance.GetPooledComponent<CircleCollider2D>(co.gameObject);
            cc.gameObject.SetActive(true);
            cc.isTrigger = true;
            var convoTrigger = cc.GetComponent<ObjectManager.ColliderCallbacks>();
            convoTrigger.onTriggerExit += _ => StopConversation();
            return co;
        }

        public virtual void StartConversation(PlayerObject po)
        {
            if (!talking && characterData!=null && characterData.dialog != null)
            {
                po.StartCoroutine(UIManager.Instance.ShowDialog(characterData.dialog, ()=>talking = false));
                talking = true;
            }
        }

        public void StopConversation()
        {
            UIManager.Instance.CloseDialog();
        }
    }
}