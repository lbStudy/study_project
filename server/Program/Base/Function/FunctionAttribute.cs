using System;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FunctionAttribute : Attribute
    {
        public int funcid { get; private set; }


        public FunctionAttribute(int funcid)
        {
            this.funcid = funcid;
        }
    }
}