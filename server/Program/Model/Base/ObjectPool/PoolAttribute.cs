using System;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PoolAttribute : Attribute
    {
        public int maxCount;
        //public int objectIndex;
        public PoolAttribute(int maxCount = 10)
        {
            this.maxCount = maxCount;
            //this.objectIndex = objectIndex;
        }
    }
}
