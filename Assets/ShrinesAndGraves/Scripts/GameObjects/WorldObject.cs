using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObject : MonoBehaviour
    {
        public delegate void ObjectEvent(WorldObject other);

        public ObjectEvent OnEnter;
        
        protected Entity m_Entity;

        protected SpriteRenderer m_renderer;
        protected Transform m_rendererTransform;

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

        public virtual void InitializeRenderers(Entity e)
        {
            if (e != null)
            {
                gameObject.name = e.name;
            }

            m_Entity = e;

            if (m_renderer == null)
            {
                var sgo = new GameObject("sprite");
                m_rendererTransform = sgo.transform;
                m_rendererTransform.SetParent(transform, false);
                m_renderer = sgo.AddComponent<SpriteRenderer>();
            }
            else
            {
                m_renderer.enabled = true;
            }
            m_renderer.sharedMaterial = ResourceManager.Instance.spriteMaterial;
            if (e != null && entity.data != null)
            {
                m_renderer.sprite = entity.data.GetSprite();
            }

            if (m_renderer.sprite != null)
            {
                m_rendererTransform.localPosition = m_renderer.sprite.pivot / m_renderer.sprite.pixelsPerUnit;
            }

            gameObject.transform.position = e.position;
        }

        public virtual void InitializeGameobject(Entity e)
        {

            if (m_Entity != null)
            {
                ResetGameobject();
            }

            InitializeRenderers(e);

            if (collider == null)
            {
                var bc = gameObject.GetOrAddComponent<BoxCollider2D>();
                bc.offset = Vector2.one * 0.5f;
                collider = bc;
            }

            if (e.IsTrigger)
            {
                collider.enabled = true;
                collider.isTrigger = true;
                OnEnter += e.OnTriggerEnter;
            }
            else
            {
                collider.enabled = false;
                collider.isTrigger = false;
                OnEnter = null;
            }
        }

        public virtual void ResetGameobject()
        {
            if (m_renderer != null)
            {
                m_renderer.enabled = false;
            }

            if (m_Entity != null)
            {
                m_Entity.viewObject = null;
            }
            
            m_Entity = null;
        }

        protected void SetRotation(float angle)
        {
            m_rendererTransform.Rotate(0, 0, angle);
        }

        protected void SetRenderer(SpriteRenderer renderer)
        {
            m_renderer = renderer;
            m_rendererTransform = renderer.GetComponent<Transform>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (OnEnter != null)
            {
                var wo = other.gameObject.GetComponent<WorldObject>();
                if (wo != null)
                {
                    OnEnter(wo);
                }
            }
        }
    }
}