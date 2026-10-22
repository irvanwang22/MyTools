using System;
using System.Collections.Generic;
using System.Text;

namespace MyTools.ObjectPool
{
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is being reused from the pool.
        /// </summary>
        public void OnPoolActivate();

        /// <summary>
        /// Called when the object is being returned to the pool.
        /// </summary>
        public void OnPoolDeactivate();
    }

}
