using System;
using System.Net;
using System.Threading.Tasks;

namespace Base
{
	public enum NetworkProtocol
	{
		TCP,
		UDP
	}

	public abstract class AService: Disposer
	{
        private Action<AChannel> acceptCallback;

        public event Action<AChannel> AcceptCallback
        {
            add
            {
                this.acceptCallback += value;
            }
            remove
            {
                this.acceptCallback -= value;
            }
        }

        protected void OnAccept(AChannel channel)
        {
            this.acceptCallback?.Invoke(channel);
        }

        public abstract AChannel GetChannel(long id);
        public abstract AChannel ConnectChannel(IPEndPoint ipEndPoint);
        public abstract void Remove(long channelId);
        public abstract void Start();
        public abstract void Update();

		public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            acceptCallback = null;
        }
	}
}