﻿using System;
using System.ComponentModel;

namespace Base
{
	public abstract class Object: ISupportInitialize
	{
		public long Id { get; protected set; }

		protected Object()
		{
			Id = IdGenerater.GenerateId();
		}

		protected Object(long id)
		{
			this.Id = id;
		}

		public virtual void BeginInit()
		{
		}

		public virtual void EndInit()
		{
		}
	}
}