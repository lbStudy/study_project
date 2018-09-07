
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Base
{
    [Pool(100)]
	public abstract class Component: Disposer, Initer
    {
        [BsonIgnore]
        public Entity Owner;

        public T GetOwner<T>() where T : Entity
		{
			return this.Owner as T;
		}
        protected Component()
        {

        }
        public void Init()
        {
            ComponentManager.Instance.Add(this);
        }

        public T GetComponent<T>() where T : Component
		{
			return this.Owner.GetComponent<T>();
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            base.Dispose();
			this.Owner.RemoveComponent(this.GetType());
            //ComponentManager.Instance.Remove(this);
        }
    }
}