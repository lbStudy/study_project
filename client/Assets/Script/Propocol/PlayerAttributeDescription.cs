using System;

namespace Data
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class PlayerAttributeDescription : Attribute
    {
        public string valType;
        public long maxVal;
        public PlayerAttributeDescription(string valType, long maxVal)
        {
            this.valType = valType;
            this.maxVal = maxVal;
        }
    }
}


