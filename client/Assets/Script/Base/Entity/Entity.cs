using System;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
	public class Entity: Disposer
	{
		public EntityType Type { get; set; }

		private Dictionary<Type, Component> componentDict = new Dictionary<Type, Component>();

		protected Entity(EntityType entityType)
		{
			this.Type = entityType;
		}

		protected Entity(long id, EntityType entityType): base(id)
		{
			this.Type = entityType;
		}

		public override void Dispose()
		{
			if (this.IsDisposer)
			{
				return;
			}

			base.Dispose();

            Component[] components = this.GetComponents();

            foreach (Component component in components)
			{
				try
				{
					component.Dispose();
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e.ToString());
				}
			}
        }

		public K AddComponent<K>() where K : Component, new()
		{
            if (this.componentDict.ContainsKey(typeof(K)))
			{
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof (K).Name}");
            }

            K component = (K)Activator.CreateInstance(typeof(K));
            component.Owner = this;
            this.componentDict.Add(component.GetType(), component);
            var awake = component as IAwake;
            if(awake != null)
                awake.Awake();
            return component;
        }

		public K AddComponent<K, P1>(P1 p1) where K : Component, new()
		{
			if (this.componentDict.ContainsKey(typeof(K)))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof (K).Name}");
			}

            K component = (K)Activator.CreateInstance(typeof(K));
            component.Owner = this;

			this.componentDict.Add(component.GetType(), component);
            var awake = component as IAwake<P1>;
            if (awake != null)
                awake.Awake(p1);
			return component;
		}

		public K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
		{
            if (this.componentDict.ContainsKey(typeof(K)))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = (K) Activator.CreateInstance(typeof (K));
			component.Owner = this;

			this.componentDict.Add(component.GetType(), component);

            var awake = component as IAwake<P1, P2>;
            if (awake != null)
                awake.Awake(p1, p2);

            return component;
		}

		public K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
		{
            if (this.componentDict.ContainsKey(typeof(K)))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = (K) Activator.CreateInstance(typeof (K));
			component.Owner = this;

			this.componentDict.Add(component.GetType(), component);

            var awake = component as IAwake<P1, P2, P3>;
            if (awake != null)
                awake.Awake(p1, p2, p3);

            return component;
		}

		public void RemoveComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof (K), out component))
			{
				return;
			}

			this.componentDict.Remove(typeof (K));

			component.Dispose();
		}

		public void RemoveComponent(Type type)
		{
			Component component;
			if (!this.componentDict.TryGetValue(type, out component))
			{
				return;
			}

			this.componentDict.Remove(type);

			component.Dispose();
		}

		public K GetComponent<K>() where K : Component
		{
			Component component;
			if (!this.componentDict.TryGetValue(typeof (K), out component))
			{
				return default(K);
			}
			return (K) component;
		}

        public Component[] GetComponents()
        {
            return componentDict.Values.ToArray();
        }
    }
}