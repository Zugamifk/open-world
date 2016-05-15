using UnityEngine;
using System.Collections;

namespace Shrines {
    public class CharacterObject : WorldObject {

        public Animator animator;
        public Transform graphicsRoot;

        public GameObject gibs;

        Character character;

        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            character = e as Character;

            if (character == null)
            {
                Debug.LogError("Can not initialize Character \'"+ gameObject.name +"\' with Entity of type " + e.GetType() + "!");
                return;
            }
        }

        protected override void SetRenderer(GameObject go)
        {
            var cp = go.GetComponent<CharacterPrefab>();
            if (cp != null)
            {
                go.transform.SetParent(transform, false);
                animator = cp.animator;
                graphicsRoot = cp.graphicRoot;
                base.SetRenderer(cp.defaultSprite);
            }
            else
            {
                base.SetRenderer(go);
            }
        }

        public virtual void Die()
        {
            animator.enabled = false;
            graphicsRoot.gameObject.SetActive(false);
            collider.enabled = false;
            m_rigidbody.isKinematic = true;

            if (gibs != null)
            {
                var go = WorldManager.SpawnObject(gibs, graphicsRoot.transform.position, graphicsRoot.transform.rotation);
                var g = go.GetComponent<Gibs>();
                g.power = 1500;
                g.enabled = true;
                go.transform.SetParent(transform, true);
            }
        }

    }
}