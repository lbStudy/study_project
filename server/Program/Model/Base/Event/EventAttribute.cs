using System;
using System.Collections.Generic;

namespace Base
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EventAttribute : Attribute
	{
		public int eventType { get; private set; }
        public List<AppType> Apphandles { get; private set; }


        public EventAttribute(int eventType, params object[] apps)
		{
			this.eventType = eventType;
            if(apps != null && apps.Length > 0)
            {
                Apphandles = new List<AppType>();
                for (int i = 0; i < apps.Length; i++)
                {
                    Apphandles.Add((AppType)apps[i]);
                }
            }
		}
	}
}