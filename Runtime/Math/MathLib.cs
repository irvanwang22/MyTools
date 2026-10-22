using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTools.Math
{
    /// <summary>
    /// A library of mathematical functions and utilities.
    /// </summary>
    public static class MathLib
    {
        /// <summary>
        /// Creates an array containing a sequence of integers within the specified range.
        /// </summary>
        /// <param name="range">The exclusive upper bound of the range. Must be greater than or equal to <paramref name="min"/>.</param>
        /// <returns>An array of integers starting from <paramref name="min"/> and ending at <paramref name="max"/>.</returns>
        /// <param name="offset">The overall offset of each element.</param>
        public static int[] MakeCountArray(int range, int offset = 0)
        {
            int[] rst = new int[range];
            for (int i = 0; i < rst.Length; i++)
            {
                rst[i] = i + offset;
            }
            return rst;
        }

        /// <summary>
        /// Randomly selects a specified number of unique integers from a given range, with an option to return them in sorted order.
        /// </summary>
        public static int[] RandomChoose(int range, int count, bool inOrder, int seed)
        {
            if (range <= 0 || count <= 0)
                return new int[0];
            if (count >= range)
                return MakeCountArray(range);
            Random random = new(seed);
            HashSet<int> set = new();
            if (!inOrder)
            {
                while (set.Count < count)
                {
                    set.Add(random.Next(0, range));
                }
                return set.ToArray();
            }
            else
            {
                while (set.Count < count)
                {
                    set.Add(random.Next(0, range));
                }
                return set.OrderBy(i => i).ToArray();
            }
        }

        /// <summary>
        /// Selects a specified number of random elements from the given list.
        /// </summary>
        /// <remarks>This method ensures that the selected elements are chosen randomly, and the selection
        /// is deterministic when the same <paramref name="seed"/> value is provided.</remarks>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="values">The list of elements to choose from. Cannot be null or empty.</param>
        /// <param name="count">The number of elements to select. Must be greater than 0 and less than or equal to the number of elements in
        /// <paramref name="values"/>.</param>
        /// <param name="inOriginalOrder">A value indicating whether the selected elements should retain their original order from the input list. If
        /// <see langword="true"/>, the elements will appear in the same order as in <paramref name="values"/>. If <see
        /// langword="false"/>, the order of the selected elements may differ.</param>
        /// <param name="seed">The seed value used to initialize the random number generator, ensuring reproducible results.</param>
        /// <returns>An array containing the randomly selected elements. The array will have a length equal to <paramref
        /// name="count"/>. If <paramref name="count"/> is 0 or <paramref name="values"/> is null or empty, an empty
        /// array is returned.</returns>
        public static T[] RandomChoose<T>(IList<T> values, int count, bool inOriginalOrder, int seed)
        {
            if (values == null || values.Count == 0 || count <= 0)
                return new T[0];
            if (count >= values.Count)
                return values.ToArray();
            int[] indices = RandomChoose(values.Count, count, inOriginalOrder, seed);
            T[] rst = new T[count];
            for (int i = 0; i < count; i++)
            {
                rst[i] = values[indices[i]];
            }
            return rst;
        }

        public static T[] RandomChoose<T>(IList<T> values, int count, bool inOriginalOrder)
        {
            int seed = Environment.TickCount;
            return RandomChoose(values, count, inOriginalOrder, seed);
        }

        public static int[] DistributeEvenly(int num, int length)
        {
            int[] rst = new int[length];
            int divLeft = num % length;
            int baseValue = num / length;
            for (int i = 0; i < length; i++)
            {
                rst[i] = baseValue + (i < divLeft ? 1 : 0);
            }
            return rst;
        }
    }
}