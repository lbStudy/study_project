using System;
using Base;
using MongoDB.Bson.Serialization.Attributes;

namespace Base
{
    //池对象必须继承该类
    public abstract class Disposer
    {
        [BsonIgnore]
        public bool IsBackPool = false;
        [BsonIgnore]
        public bool IsDisposed = false;

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (IsBackPool)
                ObjectPoolManager.Instance.Back(this);
        }
    }
}