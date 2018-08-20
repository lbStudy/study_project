using System;
using System.Collections.Generic;
using System.Reflection;


namespace Base
{
	/// <summary>
	/// 事件分发
	/// </summary>
	public class EventDispatcher
	{
		private object[] allEvents;

        private static EventDispatcher instance;
        public static EventDispatcher Instance {
            get
            {
                if (instance == null)
                    instance = new EventDispatcher();
                return instance;
            }
        }
		public void Load(Assembly assembly, AppType appType, int eventCount)
		{
            this.allEvents = new object[eventCount];
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);
                if(attrs == null || attrs.Length == 0)
                {
                    continue;
                }
                EventAttribute aEventAttribute = (EventAttribute)attrs[0];
                if (aEventAttribute.Apphandles != null && !aEventAttribute.Apphandles.Contains(appType))
                    continue;
                object obj = Activator.CreateInstance(type);
                this.allEvents[aEventAttribute.eventType] = obj;
            }
        }

		public void Run(int type)
		{
            if (this.allEvents[type] != null)
			{
                try
                {
                    IEvent iEvent = this.allEvents[type] as IEvent;
                    if (iEvent != null)
                    {
                        iEvent.Run();
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A>(int type, A a)
		{
            if (this.allEvents[type] != null)
			{
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A, B>(int type, A a, B b)
		{
            if (this.allEvents[type] != null)
			{
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A, B>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a, b);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A, B, C>(int type, A a, B b, C c)
		{
            if (this.allEvents[type] != null)
			{
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A, B, C>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a, b, c);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A, B, C, D>(int type, A a, B b, C c, D d)
		{
            if (this.allEvents[type] != null)
			{
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A, B, C, D>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a, b, c, d);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A, B, C, D, E>(int type, A a, B b, C c, D d, E e)
		{
            if (this.allEvents[type] != null)
            {
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A, B, C, D, E>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a, b, c, d, e);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }

		public void Run<A, B, C, D, E, F>(int type, A a, B b, C c, D d, E e, F f)
		{
            if (this.allEvents[type] != null)
            {
                try
                {
                    var iEvent = this.allEvents[type] as IEvent<A, B, C, D, E, F>;
                    if (iEvent != null)
                    {
                        iEvent.Run(a, b, c, d, e, f);
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err.ToString());
                }
            }
            else
            {
                Log.Warning($"Not exist {type.ToString()} event handle.");
            }
        }
	}
}