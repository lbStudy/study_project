using System;
using Base;

namespace Base
{
	public abstract class Disposer: Object, IDisposable
	{
        bool isDisposer;

        public bool IsDisposer { get { return isDisposer; } }

        protected Disposer(): base(IdGenerater.GenerateId())
		{
			
		}

		protected Disposer(long id): base(id)
		{
			
		}

		public virtual void Dispose()
		{
            this.isDisposer = true;
        }
	}
}