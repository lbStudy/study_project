using System;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtocolAttribute : Attribute
    {
        public int opCode;

        public ProtocolAttribute(int opCode)
        {
            this.opCode = opCode;
        }
    }
}
