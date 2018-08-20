using System;
using System.Collections.Generic;
using System.Reflection;


namespace Hotfix
{
	/// <summary>
	/// 事件分发
	/// </summary>
	public class EventDispatcher
	{
		private List<IEvent>[] allEvents;

        private static EventDispatcher instance;
        public static EventDispatcher Instance {
            get
            {
                if (instance == null)
                    instance = new EventDispatcher();
                return instance;
            }
        }
		public void Load(Assembly assembly, int eventCount)
		{
            this.allEvents = new List<IEvent>[eventCount];
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);
                if(attrs == null || attrs.Length == 0)
                {
                    continue;
                }
                object obj = Activator.CreateInstance(type);
                IEvent handle = obj as IEvent;
                if(handle == null)
                {
                    UnityEngine.Debug.Log($"{type.Name} 应该继承 IEvent接口才能进行事件监听。");
                    continue;
                }
                List<IEvent> ies = null;
                foreach (object val in attrs)
                {
                    EventAttribute ea = (EventAttribute)val;
                    ies = this.allEvents[ea.eventType];
                    if (ies == null)
                    {
                        ies = new List<IEvent>();
                    }
                    if (ies.Contains(handle))
                        continue;
                    ies.Add(handle);
                }
            }
        }
		public void Run(EventPackage eventPackage)
		{
            if (this.allEvents[(int)eventPackage.eventType] != null)
			{
                try
                {
                    List<IEvent> ies = this.allEvents[(int)eventPackage.eventType];
                    for(int i = 0; i < ies.Count; i++)
                    {
                        ies[i].Run(eventPackage);
                    }
                }
                catch (Exception err)
                {
                    UnityEngine.Debug.LogError(err.ToString());
                }
            }
            else
            {
                UnityEngine.Debug.Log($"Not exist {eventPackage.eventType.ToString()} event handle.");
            }
        }

	}
}