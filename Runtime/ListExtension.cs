using System;
using System.Collections.Generic;

namespace MyTools
{
    public static class ListExtension
    {
        public static bool AllEqualsTo<T>(this IList<T> list, T value)
        {
            foreach (T i in list)
            {
                if (!i.Equals(value))
                    return false;
            }
            return true;
        }

        public static int CountEqualsTo<T>(this IList<T> list, T value)
        {
            int count = 0;
            foreach (T i in list)
            {
                if (i.Equals(value))
                    count++;
            }
            return count;
        }

        public static int CountTrue<T>(this IList<T> list, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (T i in list)
            {
                if (predicate(i))
                    count++;
            }
            return count;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void SeededShuffle<T>(this IList<T> list, int seed)
        {
            Random random = new(seed);
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default;
            T rst = list[0];
            list.RemoveAt(0);
            return rst;
        }

        public static T PopAdd<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default;
            T rst = list[0];
            list.RemoveAt(0);
            list.Add(rst);
            return rst;
        }

        public static T RemoveLast<T>(this IList<T> list)
        {
            T rst = list[list.Count - 1];
            list.Remove(rst);
            return rst;
        }

        public static T RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    T rst = list[i];
                    list.RemoveAt(i);
                    return rst;
                }
            }
            return default;
        }

        public static int RemoveAllWhere<T>(this IList<T> list, Func<T, bool> predicate)
        {
            int count = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    count++;
                }
            }
            return count;
        }

        public static T Last<T>(this IList<T> list, int index = 0)
        {
            return list[list.Count - 1 - index];
        }
    }
}