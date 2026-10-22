using System;

namespace MyTools
{ 
    public static class EnumLib<T> where T : Enum
    {
        public static int Count()
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        public static bool SafeIncrement(ref T e)
        {
            int rst = Convert.ToInt32(e) + 1;
            if (Enum.IsDefined(typeof(T), rst))
            {
                e = (T)Enum.ToObject(typeof(T), rst);
                return true;
            }
            return false;
        }

        public static bool SafeDecrement(ref T e)
        {
            int rst = Convert.ToInt32(e) - 1;
            if (Enum.IsDefined(typeof(T), rst))
            {
                e = (T)Enum.ToObject(typeof(T), rst);
                return true;
            }
            return false;
        }
    }
}