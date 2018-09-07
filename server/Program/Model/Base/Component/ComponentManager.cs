using MongoDB.Driver;
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

		private readonly List<Component> addList = new List<Component>();

        private readonly List<Type> dbTypes = new List<Type>();

        private readonly Dictionary<Type, List<DBComponent>> DBdDic = new Dictionary<Type, List<DBComponent>>();
       

		public void Add(Component component)
		{
            if ((component as IUpdate) != null)
            {
                this.addList.Add(component);
            }
            else if((component as DBComponent) != null)
            {
                this.addList.Add(component);
            }
        }

		private void AddComponent()
		{
            if (this.addList.Count == 0) return;

            IUpdate iupdate = null;
            DBComponent dbcomponent = null;
			foreach (Component component in this.addList)
			{
                iupdate = component as IUpdate;
                dbcomponent = component as DBComponent;
                if ((iupdate) != null && updateComponents.Contains(iupdate) == false)
                    updateComponents.Add(iupdate);
                if(dbcomponent != null)
                {
                    List<DBComponent> dbs = null;
                    Type type = dbcomponent.GetType();
                    if(DBdDic.TryGetValue(type, out dbs) == false)
                    {
                        dbs = new List<DBComponent>();
                        dbTypes.Add(type);
                        DBdDic.Add(type, dbs);
                    }
                    if(dbs.Contains(dbcomponent) == false)
                    {
                        dbs.Add(dbcomponent);
                    }
                }
			}

			this.addList.Clear();
		}
        List<WriteModel<DBComponent>> bulkWrite = new List<WriteModel<DBComponent>>();
        int typeIndex = 0;
        int saveIndex = 0;
        List<DBComponent> saveDBs = new List<DBComponent>();
        public void Update()
        {
            AddComponent();
            Component component;
            for (int i = updateComponents.Count; i >= 0; i--)
            {
                component = updateComponents[i] as Component;
                if (component.IsDisposed)
                {
                    updateComponents.RemoveAt(i);
                    continue;
                }
                try
                {
                    updateComponents[i].Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
        }



        public void WriteDB()
        {
            DBComponent dbComponent = null;
            foreach (Type type in dbTypes)
            {
                List<DBComponent> dbs = DBdDic[type];
                if (dbs.Count > 0)
                {
                    for (int i = dbs.Count; i >= 0; i--)
                    {
                        dbComponent = dbs[i];
                        if (dbComponent.IsDisposed)
                        {
                            dbs.RemoveAt(i);
                            continue;
                        }
                        if (dbComponent.isSave)
                            bulkWrite.Add(dbComponent.GetReplaceOneModel());
                    }
                }
            }
        }
    }
}