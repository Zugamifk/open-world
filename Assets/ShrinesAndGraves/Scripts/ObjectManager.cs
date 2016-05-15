using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class ObjectManager : MonoBehaviour
    {
        class PoolObject<T> where T : Component
        {
            public GameObject root;
            public T component;
            public ComponentPool<T> pool;
            public List<PoolObjectComponent> extraComponents;

            public PoolObject(GameObject target, ComponentPool<T> pool)
            {
                root = target;
                component = target.AddComponent<T>();
                extraComponents = new List<PoolObjectComponent>();
                this.pool = pool;
                Return();
            }

            public T AddPoolObjectComponent<T>() where T : PoolObjectComponent
            {
                var poc = root.AddComponent<T>();
                extraComponents.Add(poc);
                return poc;
            }

            public void Return()
            {
                root.SetActive(false);

                for(int i=0;i<extraComponents.Count;i++) {
                    extraComponents[i].Reset();
                }

                pool.Return(this);
            }
        }
        class ComponentPool
        {
            protected Transform root;
        }
        class ComponentPool<T> : ComponentPool where T : Component
        {
            protected Queue<PoolObject<T>> pool;

            public ComponentPool(Transform parent, int size)
            {
                string name = typeof(T).ToString();
                var go = new GameObject(name+" pool");
                root = go.transform;
                root.SetParent(parent);

                pool = new Queue<PoolObject<T>>();
                for (int i = 0; i < size; i++)
                {
                    var cgo = new GameObject(name+" " + i);
                    var po = new PoolObject<T>(cgo, this);
                    pool.Enqueue(po);
                }
            }

            public void ProcessPool(System.Action<PoolObject<T>> action)
            {
                foreach (var p in pool)
                {
                    action(p);
                }
            }

            public PoolObject<T> GetComponent()
            {
                return pool.Dequeue();
            }

            public void Return(PoolObject<T> c)
            {
                pool.Enqueue(c);
                c.component.transform.SetParent(root, false);
            }
        }

        public abstract class PoolObjectComponent : MonoBehaviour
        {
            public abstract void Reset();
        }
        public delegate void ColliderCallback(Collider2D other);
        public class ColliderCallbacks : PoolObjectComponent
        {
            public ObjectManager.ColliderCallback onTriggerEnter, onTriggerStay, onTriggerExit;

            public override void Reset()
            {
                onTriggerEnter = null;
                onTriggerStay = null;
                onTriggerExit = null;
            }

            void OnTriggerEnter2D(Collider2D other)
            {
                if (onTriggerEnter != null)
                {
                    onTriggerEnter.Invoke(other);
                }
            }

            void OnTriggerStay2D(Collider2D other)
            {
                if (onTriggerStay != null)
                {
                    onTriggerStay.Invoke(other);
                }
            }

            void OnTriggerExit2D(Collider2D other)
            {
                Debug.Log("!");
                if (onTriggerExit != null)
                {
                    onTriggerExit.Invoke(other);
                }
            }
        }

        public Transform poolRoot;
        public int CircleColliderCount;

        public int CharacterCount;
        public int WorldObjectCount;

        List<GameObject> spawnedObjects;
        List<GameObject> tempObjects;

        public delegate void ReturnDelegate();
        Dictionary<GameObject, ReturnDelegate> returnCallbacks;

        Dictionary<System.Type, ComponentPool> poolLookup;

        public static ObjectManager Instance;

        public T GetPooledComponent<T>() where T : Component
        {
            if (!poolLookup.ContainsKey(typeof(T)))
            {
                Debug.Log("No object pool of type " + typeof(T).ToString());
                return null;
            }
            var pool = poolLookup[typeof(T)] as ComponentPool<T>;
            var c = pool.GetComponent();

            ReturnDelegate rd;
            var go = c.root;
            if (returnCallbacks.TryGetValue(go, out rd))
            {
                rd += c.Return;
            }
            else
            {
                returnCallbacks[go] = c.Return;
            }

            return c.component;
        }

        public T GetPooledComponent<T>(GameObject go) where T : Component
        {
            if (!poolLookup.ContainsKey(typeof(T)))
            {
                Debug.Log("No object pool of type " + typeof(T).ToString());
                return null;
            }
            ComponentPool<T> pool = poolLookup[typeof(T)] as ComponentPool<T>;
            var c = pool.GetComponent();

            ReturnDelegate rd;
            if (returnCallbacks.TryGetValue(go, out rd))
            {
                rd += c.Return;
            }
            else
            {
                returnCallbacks[go] = c.Return;
            }

            c.root.transform.SetParent(go.transform, false);

            return c.component;
        }

        public void ReturnComponents(GameObject go)
        {
            ReturnDelegate rd;
            if (returnCallbacks.TryGetValue(go, out rd))
            {
                rd.Invoke();
                returnCallbacks.Remove(go);
            }
        }

        void Awake()
        {
            if (!this.SetInstanceOrKill(ref Instance))
            {
                return;
            }

            spawnedObjects = new List<GameObject>();
            tempObjects = new List<GameObject>();

            returnCallbacks = new Dictionary<GameObject, ReturnDelegate>();

            poolLookup = new Dictionary<System.Type, ComponentPool>();

            var ccp = new ComponentPool<CircleCollider2D>(poolRoot, CircleColliderCount);
            ccp.ProcessPool(cc => cc.AddPoolObjectComponent<ColliderCallbacks>());
            poolLookup.Add(typeof(CircleCollider2D), ccp);

            var wop = new ComponentPool<WorldObject>(poolRoot, WorldObjectCount);
            poolLookup.Add(typeof(WorldObject), wop);

            var cop = new ComponentPool<CharacterObject>(poolRoot, CharacterCount);
            poolLookup.Add(typeof(CharacterObject), cop);
        }
    }
}