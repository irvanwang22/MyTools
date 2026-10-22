using System.Collections.Generic;

namespace MyTools
{
    /// <summary>
    /// A list that has a maximum size. When the size is exceeded, it removes items based on a defined strategy (default is the first object in the list).
    /// The implementation of the removal strategy can be customized by overriding the GetDefaultRemoveObject method.
    /// Caution: the use of null values in this list is not recommended, as it may lead to unexpected behavior because of the removal strategy.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaxList<T> : List<T>
    {
        public int maxSize { get; private set; } = 1;

        public MaxList(int maxSize) : base()
        {
            this.maxSize = maxSize;
        }

        /// <summary>
        /// Adds an item to the list. 
        /// If the list exceeds its maximum size, it removes items based on the defined strategy until the size is within the limit.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The last removed item, if multiple items removed. 
        /// Or default(T) if no item was removed.</returns>
        public new T Add(T item)
        {
            T lastRemoved = default;
            while (Count >= maxSize)
            {
                lastRemoved = GetDefaultRemoveObject();
                if (!Remove(lastRemoved))
                    break;
            }
            base.Add(item);
            return lastRemoved;
        }

        /// <summary>
        /// Gets the default object to remove when the list exceeds its maximum size.
        /// Override this method to customize the removal behavior.
        /// return null if no object should be removed.
        /// </summary>
        public virtual T GetDefaultRemoveObject()
        {
            return this[0];
        }
    }
}