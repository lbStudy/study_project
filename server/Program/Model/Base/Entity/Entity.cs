using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base
{
    [Pool(100)]
    public abstract class Entity: Disposer, Initer<long>
	{
		public EntityType EType { get { return eType; } }
        private Dictionary<Type, Component> componentDict = null;
        private long id;
        public long Id { get { return id; } }
        protected EntityType eType;

        protected Entity(EntityType entityType)
		{
			this.eType = entityType;
		}
        public void Init(long id)
        {
            this.id = id;
            EntityManager.Instance.Add(this);
        }
        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            base.Dispose();
            if (componentDict != null)
            {
                Component[] components = this.GetComponents();

                foreach (Component component in components)
                {
                    try
                    {
                        component.Dispose();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
                componentDict.Clear();
            }
            EntityManager.Instance.Remove(this);
        }
        public async Task<K> AddDBComponent<K>() where K : DBComponent, new()
        {
            Type type = typeof(K);
            if (this.componentDict != null && this.componentDict.ContainsKey(type))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {type.Name}");
            }

            K component = await DBOperateComponent.Instance.FindOneComponen<K>(id);
            if(component == null)
            {
                component = Activator.CreateInstance(type) as K;
                component.id = id;
            }
            if(componentDict.ContainsKey(type))
            {
                return componentDict[type] as K;
            }
            AddComponent(component);
            component.Init();
            var awake = component as IAwake;
            if (awake != null)
                awake.Awake();
            return component;
        }
        public K AddComponent<K>() where K : Component, new()
		{
            if (this.componentDict != null && this.componentDict.ContainsKey(typeof(K)))
			{
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof (K).Name}");
            }

            K component = ObjectPoolManager.Instance.Take<K>();
            AddComponent(component);

            var awake = component as IAwake;
            if(awake != null)
                awake.Awake();
            return component;
        }

		public K AddComponent<K, P1>(P1 p1) where K : Component, new()
		{
			if (this.componentDict != null && this.componentDict.ContainsKey(typeof(K)))
			{
				throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof (K).Name}");
			}

            K component = ObjectPoolManager.Instance.Take<K>();
            AddComponent(component);

            var awake = component as IAwake<P1>;
            if (awake != null)
                awake.Awake(p1);
			return component;
		}

		public K AddComponent<K, P1, P2>(P1 p1, P2 p2) where K : Component, new()
		{
            if (this.componentDict != null && this.componentDict.ContainsKey(typeof(K)))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ObjectPoolManager.Instance.Take<K>();
            AddComponent(component);

            var awake = component as IAwake<P1, P2>;
            if (awake != null)
                awake.Awake(p1, p2);

            return component;
		}

		public K AddComponent<K, P1, P2, P3>(P1 p1, P2 p2, P3 p3) where K : Component, new()
		{
            if (this.componentDict != null && this.componentDict.ContainsKey(typeof(K)))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ObjectPoolManager.Instance.Take<K>();
            AddComponent(component);

            var awake = component as IAwake<P1, P2, P3>;
            if (awake != null)
                awake.Awake(p1, p2, p3);

            return component;
		}
        public K AddComponent<K, P1, P2, P3, P4>(P1 p1, P2 p2, P3 p3, P4 p4) where K : Component, new()
        {
            if (this.componentDict != null && this.componentDict.ContainsKey(typeof(K)))
            {
                throw new Exception($"AddComponent, component already exist, id: {this.Id}, component: {typeof(K).Name}");
            }

            K component = ObjectPoolManager.Instance.Take<K>();
            AddComponent(component);

            var awake = component as IAwake<P1, P2, P3, P4>;
            if (awake != null)
                awake.Awake(p1, p2, p3, p4);

            return component;
        }
        void AddComponent(Component component)
        {
            component.Owner = this;
            if (this.componentDict == null)
                this.componentDict = new Dictionary<Type, Component>();
            this.componentDict.Add(component.GetType(), component);
        }
        public void RemoveComponent<K>() where K : Component
		{
            if (componentDict == null)
                return;
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
            if (componentDict == null)
                return;
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
            if (componentDict == null)
                return null;
            Component component;
			if (!this.componentDict.TryGetValue(typeof (K), out component))
			{
				return default(K);
			}
			return (K) component;
		}

        public Component[] GetComponents()
        {
            if (componentDict == null)
                return null;
            return componentDict.Values.ToArray();
        }
    }
}