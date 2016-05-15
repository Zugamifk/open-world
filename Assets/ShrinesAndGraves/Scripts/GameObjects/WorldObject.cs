using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObject : MonoBehaviour
    {
        public delegate void ObjectEvent(WorldObject other);

        public ObjectEvent OnEnter;
        
        protected Entity m_Entity;

        // the default renderer to use, saved here when overriding with a prefab
        SpriteRenderer m_baseRenderer;
        protected SpriteRenderer m_renderer;
        protected Transform m_rendererTransform;
        protected Rigidbody2D m_rigidbody;


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
                m_baseRenderer = m_renderer;
            }
            else
            {
                m_renderer.enabled = true;
            }

            m_renderer.sharedMaterial = ResourceManager.Instance.spriteMaterial;
            if (e != null && entity.data != null)
            {
                if (entity.data.graphicsPrefab != null)
                {
                    var graphics = (GameObject)Instantiate(entity.data.graphicsPrefab);
                    SetRenderer(graphics);
                }
                else
                {
                    m_renderer.sprite = entity.data.GetSprite();
                }
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

            gameObject.name = e.name;

            if (collider == null)
            {
                var bc = gameObject.GetOrAddComponent<BoxCollider2D>();
                bc.offset = Vector2.one * 0.5f;
                collider = bc;
            }

            if (m_rigidbody == null)
            {
                var rb = gameObject.GetOrAddComponent<Rigidbody2D>();
                m_rigidbody = rb;
            }

            m_rigidbody.isKinematic = !e.canMove;

            if (e.collides)
            {
                collider.enabled = true;
                if (e.IsTrigger)
                {
                    collider.isTrigger = true;
                    OnEnter += e.OnTriggerEnter;
                }
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

            if (m_baseRenderer != m_renderer)
            {
                Destroy(m_renderer.gameObject);
                m_renderer = m_baseRenderer;
            }

            if (collider != null)
            {
                collider.enabled = false;
                collider.isTrigger = false;
                OnEnter = null;
            }

            if (m_rigidbody != null)
            {
                m_rigidbody.isKinematic = true;
            }

            m_Entity = null;
        }

        protected void SetRotation(float angle)
        {
            m_rendererTransform.Rotate(0, 0, angle);
        }

        protected virtual void SetRenderer(SpriteRenderer renderer)
        {
            m_renderer = renderer;
            m_rendererTransform = renderer.GetComponent<Transform>();
        }

        protected virtual void SetRenderer(GameObject go)
        {
            go.transform.SetParent(m_rendererTransform.parent, false);
            m_renderer.enabled = false;
            m_rendererTransform = go.transform;
            m_renderer = go.GetComponent<SpriteRenderer>();
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