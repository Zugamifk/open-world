using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObject : MonoBehaviour
    {
        protected Entity m_Entity;

        new protected SpriteRenderer renderer;

        new protected Collider2D collider;

        public Vector2 position
        {
            get
            {
                return entity.position;
            }
            set
            {
                entity.position = value;
            }
        }

        public Vector2i positioni
        {
            get
            {
                return entity.position;
            }
            set
            {
                entity.position = value;
            }
        }

        public Entity entity
        {
            get
            {
                return m_Entity;
            }
            set
            {
                InitializeGameobject(value);
            }
        }

        public virtual void InitializeGameobject(Entity e)
        {
            if (m_Entity != null)
            {
                ResetGameobject();
            }
            if (e != null)
            {
                gameObject.name = e.name;
            }
            m_Entity = e;
            if (renderer == null)
            {
                var sgo = new GameObject("sprite");
                sgo.transform.SetParent(transform, false);
                renderer = sgo.AddComponent<SpriteRenderer>();
            } else {
                renderer.enabled = true;
            }
            renderer.sharedMaterial = ResourceManager.Instance.spriteMaterial;
            if (e!=null && entity.data != null )
            {
                renderer.sprite = entity.data.GetSprite();
            }
        }

        public virtual void ResetGameobject()
        {
            renderer.enabled = false;
            if (m_Entity != null)
            {
                m_Entity.viewObject = null;
            }
            m_Entity = null;
        }

    }
}