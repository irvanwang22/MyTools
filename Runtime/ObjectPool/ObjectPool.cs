using System.Collections.Generic;
using UnityEngine;

namespace MyTools.ObjectPool
{
    /// <summary>
    /// An object pool for managing GameObjects to optimize performance by reusing instances.
    /// Will keep at least one inactive object at all times to avoid instantiation delays.
    /// </summary>
    public class ObjectPool
    {
        /// <summary>
        /// The prefab used to instantiate new objects in the pool.
        /// </summary>
        public GameObject prefab { get; private set; }
        /// <summary>
        /// This is the soft maximum number of objects to keep in the pool. 
        /// Soft meaning that the actual size will still comply to get requests.
        /// </summary>
        public int poolSize;
        /// <summary>
        /// The default parent transform for instantiated objects.
        /// </summary>
        public Transform defaultParent;

        /// <summary>
        /// List of currently active objects in the pool.
        /// </summary>
        public List<GameObject> activePool = new();
        /// <summary>
        /// List of currently inactive objects in the pool. 
        /// The pool will try to maintain at least one inactive object to avoid instantiation delays.
        /// </summary>
        public List<GameObject> inactivePool = new();

        /// <summary>
        /// Constructor for the ObjectPool class.
        /// </summary>
        /// <param name="prefab">The prefab used to instantiate new objects in the pool.</param>
        /// <param name="poolSize">The soft maximum number of objects to keep in the pool.</param>
        /// <param name="startAmount">The initial number of objects to create in the pool.</param>
        /// <param name="defaultParent">The default parent transform for instantiated and returned objects. </param>
        public ObjectPool(GameObject prefab, int poolSize, int startAmount, Transform defaultParent)
        {
            this.prefab = prefab;
            this.poolSize = poolSize;
            this.defaultParent = defaultParent;

            InstantiateNewInactive(defaultParent);
            for (; inactivePool.Count < startAmount;)
            {
                InstantiateNewInactive(defaultParent);
            }
        }

        public GameObject GetObject()
        {
            return GetObject(defaultParent);
        }

        public GameObject GetObject(Transform parent)
        {
            GameObject rst;
            rst = FindFirstInactive();
            if (rst == null)
            {
                Debug.LogWarning("No inactive objects available in the pool. Instantiating a new one.");
                InstantiateNewInactive(defaultParent);
                InstantiateNewInactive(defaultParent);
                rst = FindFirstInactive();
            }
            rst.transform.SetParent(parent, false);
            SetObjectState(rst, true);
            return rst;
        }

        public T GetPoolableObject<T>() where T : MonoBehaviour, IPoolable
        {
            return GetPoolableObject<T>(defaultParent);
        }

        public T GetPoolableObject<T>(Transform parent) where T : MonoBehaviour, IPoolable
        {
            GameObject obj = GetObject(parent);
            T component =  obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"The object {obj.name} does not have a component of type {typeof(T).Name}.");
                ReturnObject(obj);
                return null;
            }
            return component;
        }

        public IPoolable[] GetPoolableComponents(GameObject obj)
        {
            return obj.GetComponents<IPoolable>();
        }

        public void ReturnObject(GameObject obj)
        {
            if (activePool.Contains(obj))
            {
                SetObjectState(obj, false);
                ClearExceedingInactive();
            }
            else if (inactivePool.Contains(obj))
            {
                Debug.LogWarning("Trying to return an object that is already inactive.");
            }
            else
            {
                Debug.LogError("Trying to return an object that is not part of the pool.");
            }
        }

        public void ReturnAllActive()
        {
            for (int i = activePool.Count - 1; i >= 0; i--)
            {
                ReturnObject(activePool[i]);
            }
        }

        protected virtual void SetObjectState(GameObject obj, bool state)
        {
            if (state)
            {
                inactivePool.Remove(obj);
                activePool.Add(obj);
                obj.SetActive(true);
                foreach (IPoolable p in obj.GetComponents<IPoolable>())
                {
                    p.OnPoolActivate();
                }
            }
            else
            {
                foreach (IPoolable p in obj.GetComponents<IPoolable>())
                {
                    p.OnPoolDeactivate();
                }
                activePool.Remove(obj);
                inactivePool.Add(obj);
                obj.SetActive(false);
                obj.transform.SetParent(defaultParent);
            }
        }

        private void ClearExceedingInactive()
        {
            while (activePool.Count + inactivePool.Count > poolSize && inactivePool.Count > 1)
            {
                if (!DestroyFirstInactive())
                {
                    break;
                }
            }
        }

        protected virtual GameObject InstantiateNewInactive(Transform parent)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            foreach (BaseMonoPoolable p in obj.GetComponents<BaseMonoPoolable>())
            {
                p.SetPool(this);
            }
            SetObjectState(obj, false);
            return obj;
        }

        private bool DestroyFirstInactive()
        {
            GameObject rst = FindFirstInactive();
            if (rst != null)
            {
                inactivePool.Remove(rst);
                Object.Destroy(rst);
                return true;
            }
            return false;
        }

        private GameObject FindFirstInactive()
        {
            if (inactivePool.Count > 0)
            {
                return inactivePool[0];
            }
            return null;
        }
    }
}