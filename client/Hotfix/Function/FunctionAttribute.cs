using System;

namespace Hotfix
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