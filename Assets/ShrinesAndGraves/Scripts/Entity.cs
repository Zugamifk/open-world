using UnityEngine;
using System.Collections;

namespace Shrines
{
    [System.Serializable]
    public class Entity
    {
        public Vector2f16 position;
        public WorldObjectData data;
        public Rect rect;
        public Sprite sprite;

        protected string _name = "entity";
        public virtual string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = name;
            }
        }

        /// <summary>
        /// Does this entity require a trigger?
        /// </summary>
        public bool IsTrigger
        {
            get
            {
                return data!=null && data.isTrigger;
            }
        }

        public bool canMove
        {
            get;
            protected set;
        }

        bool _collides;
        public bool collides
        {
            get
            {
                return _collides || IsTrigger;
            }
            set
            {
                _collides = value;
            }
        }

        [System.NonSerialized]
        public WorldObject viewObject;

        public Entity() {
            canMove = false;
        }

        public Entity(WorldObjectData data, Vector2f16 position) : this()
        {
            this.data = data;
            name = data.name;
            sprite = data.GetSprite();
            this.position = position;
            this.rect = new Rect(position, sprite.Size());
        }

        public virtual void OnTriggerEnter(WorldObject wo) { Debug.Log("nooo!"); }
    }
}