using System;

namespace Base
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DBAttribute : Attribute
    {
        public string tableName;

        public DBAttribute(string tableName)
        {
            this.tableName = tableName;

        }
    }
}