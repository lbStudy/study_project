using System;
using System.Collections.Generic;

namespace Hotfix
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EventAttribute : Attribute
	{
		public int eventType { get; private set; }


        public EventAttribute(int eventType)
		{
			this.eventType = eventType;
		}
	}
}