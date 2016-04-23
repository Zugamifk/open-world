using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObject : MonoBehaviour
    {
        protected Entity m_Entity;

        new protected SpriteRenderer m_renderer;

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

        public Renderer renderer
        {
            get
            {
                return m_renderer;
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
            if (m_renderer == null)
            {
                var sgo = new GameObject("sprite");
                sgo.transform.SetParent(transform, false);
                m_renderer = sgo.AddComponent<SpriteRenderer>();
            } else {
                m_renderer.enabled = true;
            }
            m_renderer.sharedMaterial = ResourceManager.Instance.spriteMaterial;
            if (e!=null && entity.data != null )
            {
                m_renderer.sprite = entity.data.GetSprite();
            }
            gameObject.transform.position = e.position;
        }

        public virtual void ResetGameobject()
        {
            m_renderer.enabled = false;
            if (m_Entity != null)
            {
                m_Entity.viewObject = null;
            }
            m_Entity = null;
        }
    }
}