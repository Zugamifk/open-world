﻿using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldObject : MonoBehaviour
    {
        protected Entity m_Entity;

        new protected SpriteRenderer renderer;

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

        public Vector3i positioni
        {
            get
            {
                return (Vector3i)entity.position;
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
        }

        public virtual void ResetGameobject()
        {
            renderer.enabled = false;
            m_Entity.viewObject = null;
            m_Entity = null;
        }
    }
}