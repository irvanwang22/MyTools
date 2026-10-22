using UnityEngine;

namespace MyTools.ObjectPool
{
    public abstract class BaseMonoPoolable : MonoBehaviour, IPoolable
    {
        public ObjectPool pool { get; private set; }

        public void SetPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public abstract void OnPoolActivate();

        public abstract void OnPoolDeactivate();

        public void ReturnToPool()
        {
            if (pool != null)
            {
                pool.ReturnObject(this.gameObject);
            }
            else
            {
                Debug.LogWarning("This object is not associated with any pool.");
                Destroy(this.gameObject);
            }
        }
    }
}