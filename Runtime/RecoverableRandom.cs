using System;
using System.Runtime.Serialization;

namespace MyTools
{
    /// <summary>
    /// A Random class that can be reset to a previous state. 
    /// Implements ISerializable for easy serialization.
    /// </summary>
    [Serializable]
    public class RecoverableRandom : Random, ISerializable
    {
        public int seed { get; private set; }
        public int useCount { get; private set; }

        public RecoverableRandom() : base() 
        {
            this.useCount = 0;
        }

        public RecoverableRandom(int seed) : base(seed) 
        {
            this.seed = seed;
            this.useCount = 0;
        }

        public RecoverableRandom(int seed, int useCount) : base(seed) 
        {
            this.seed = seed;
            this.useCount = useCount;
            for (int i = 0; i < useCount; i++)
            {
                this.Next();
            }
        }

        protected RecoverableRandom(SerializationInfo info, StreamingContext context)
            : this(info.GetInt32("seed"), info.GetInt32("useCount"))
        {
            
        }

        public override int Next()
        {
            useCount++;
            return base.Next();
        }

        public override int Next(int maxValue)
        {
            useCount++;
            return base.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            useCount++;
            return base.Next(minValue, maxValue);
        }

        public override void NextBytes(byte[] buffer)
        {
            useCount++;
            base.NextBytes(buffer);
        }

        public override double NextDouble()
        {
            useCount++;
            return base.NextDouble();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("seed", seed);
            info.AddValue("useCount", useCount);
        }
    }
}