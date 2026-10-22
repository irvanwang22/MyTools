using UnityEngine;

namespace MyTools
{
    public struct LoopInt
    {
        public int value { get; private set; }
        public int min;
        public int max;

        public LoopInt(int value, int min, int max)
        {
            if (min > max)
                throw new System.ArgumentException("min must be less than or equal to max.");
            if (value < min || value > max)
                throw new System.ArgumentOutOfRangeException(nameof(value), $"value must be between {min} and {max}.");
            this.value = value;
            this.min = min;
            this.max = max;
        }

        public LoopInt(int min, int max) : this(min, min, max) { }

        public void SetValue(int newValue)
        {
            if (newValue < min || newValue > max)
                throw new System.ArgumentOutOfRangeException(nameof(newValue), $"newValue must be between {min} and {max}.");
            value = newValue;
        }

        public bool TrySetValue(int newValue)
        {
            if (newValue < min || newValue > max)
                return false;
            value = newValue;
            return true;
        }

        public int Increment()
        {
            value++;
            if (value > max)
                value = min;
            return value;
        }

        public int Decrement()
        {
            value--;
            if (value < min)
                value = max;
            return value;
        }

        public static LoopInt operator ++(LoopInt a)
        {
            a.Increment();
            return a;
        }

        public static LoopInt operator --(LoopInt a)
        {
            a.Decrement();
            return a;
        }
    }
}