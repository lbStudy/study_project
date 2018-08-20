using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Base
{
    [Pool]
    public class ContextPackage : Disposer
    {
        public SendOrPostCallback cb;
        public object state;

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            state = null;
            cb = null;
        }
    }

    public class OneThreadSynchronizationContext : SynchronizationContext
	{
		// 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
		private readonly ConcurrentQueue<ContextPackage> queue = new ConcurrentQueue<ContextPackage>();
        static OneThreadSynchronizationContext instance;
        public static OneThreadSynchronizationContext Instance {
            get
            {
                if (instance == null)
                    instance = new OneThreadSynchronizationContext();
                return instance;
            }
        }

		private void Add(ContextPackage cp)
		{
			this.queue.Enqueue(cp);
		}

		public void Update()
		{
			while (true)
			{
                lock (queue)
                {
                    ContextPackage cp = null;
                    if (!this.queue.TryDequeue(out cp))
                    {
                        return;
                    }
                    try
                    {
                        cp.cb(cp.state);
                    }
                    finally
                    {
                        cp.Dispose();
                    }
                }
			}
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
            lock(queue)
            {
                ContextPackage cp = ObjectPoolManager.Instance.Take<ContextPackage>();
                cp.cb = callback;
                cp.state = state;
                this.Add(cp);
            }
		}
	}
}
