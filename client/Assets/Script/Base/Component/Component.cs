
namespace Base
{
	public abstract class Component: Disposer
	{
		public Entity Owner { get; set; }

		public T GetOwner<T>() where T : Entity
		{
			return this.Owner as T;
		}

		protected Component()
		{
            ComponentEventManager.Instance.Add(this);
		}

		protected Component(long id): base(id)
		{
            ComponentEventManager.Instance.Add(this);
        }
        public T GetComponent<T>() where T : Component
		{
			return this.Owner.GetComponent<T>();
		}

		public override void Dispose()
		{
			if (this.IsDisposer)
			{
				return;
			}

			base.Dispose();

			this.Owner.RemoveComponent(this.GetType());
            ComponentEventManager.Instance.Remove(this);
        }
	}
}