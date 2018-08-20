using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Base
{
	[Flags]
	public enum PacketFlags
	{
		None = 0,
		Reliable = 1 << 0,
		Unsequenced = 1 << 1,
		NoAllocate = 1 << 2
	}

	public enum ChannelType
	{
		Connect,
		Accept,
	}

	public abstract class AChannel: Disposer
	{
        public long Id { get; protected set; }

		public ChannelType ChannelType { get; protected set; }

		protected AService service;
        public int Error { get; set; }
        public IPEndPoint RemoteAddress { get; protected set; }

        private Action<AChannel, int> errorCallback;

        public event Action<AChannel, int> ErrorCallback
        {
            add
            {
                this.errorCallback += value;
            }
            remove
            {
                this.errorCallback -= value;
            }
        }

        private Action<Packet> readCallback;

        public event Action<Packet> ReadCallback
        {
            add
            {
                this.readCallback += value;
            }
            remove
            {
                this.readCallback -= value;
            }
        }

        protected void OnRead(Packet packet)
        {
            this.readCallback?.Invoke(packet);
        }

        protected void OnError(int e)
        {
            this.Error = e;
            this.errorCallback?.Invoke(this, e);
        }
		protected void Init(long id, AService service, ChannelType channelType)
        {
            this.Id = id;
            this.ChannelType = channelType;
            this.service = service;
        }
		/// <summary>
		/// 发送消息
		/// </summary>
		public abstract void Send(byte[] buffer);
        public abstract void Send(byte[] buffer, int dataLength);
        public abstract void Send(List<byte[]> buffers);
        public abstract void Send(MemoryStream stream);
        public abstract void Start();
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            base.Dispose();
            this.service.Remove(Id);
            errorCallback = null;
            readCallback = null;
            service = null;
            Error = 0;
            RemoteAddress = null;
        }
	}
}