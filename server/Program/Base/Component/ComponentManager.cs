using System;
using System.Collections.Generic;

namespace Base
{
    public sealed class ComponentManager
	{
        private static ComponentManager instance;
        public static ComponentManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ComponentManager();
                }
                return instance;
            }
        }

        private readonly List<IUpdate> updateComponents = new List<IUpdate>();

		private readonly List<IUpdate> addUpdates = new List<IUpdate>();
		private readonly List<IUpdate> removeUpdates = new List<IUpdate>();


		public void Add(Component component)
		{
            IUpdate update = component as IUpdate;
            if (update != null)
            {
                this.addUpdates.Add(update);
            }
        }

		public void Remove(Component component)
		{
            IUpdate update = component as IUpdate;
            if (update != null)
            {
                this.removeUpdates.Add(update);
            }
        }

		private void UpdateAddComponent()
		{
            if (this.addUpdates.Count == 0) return;

			foreach (IUpdate component in this.addUpdates)
			{
                updateComponents.Add(component);
			}

			this.addUpdates.Clear();
		}

		private void UpdateRemoveComponent()
		{
            if (this.removeUpdates.Count == 0) return;

            foreach (IUpdate component in this.removeUpdates)
			{
                updateComponents.Remove(component);
			}

			this.removeUpdates.Clear();
		}

		public void Update()
		{
			UpdateAddComponent();
			UpdateRemoveComponent();

			foreach (IUpdate component in updateComponents)
			{
				try
				{
					if (this.removeUpdates.Contains(component))
					{
						continue;
					}
                    component.Update();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}