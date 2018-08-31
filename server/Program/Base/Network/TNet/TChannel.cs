using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Base
{
    [Pool]
	public class TChannel : AChannel, Initer<long, IPEndPoint, TService>, Initer<long, Socket, TService>
	{
        private Socket socket;
        private SocketAsyncEventArgs innArgs;
        private SocketAsyncEventArgs outArgs;
        public readonly byte[] cache = new byte[2];

		private readonly Buffer recvBuffer = new Buffer();
		private readonly Buffer sendBuffer = new Buffer();
        private readonly PacketParser parser = new PacketParser();

        private bool isSending;
		private bool isConnected;


        public void Init(long id, IPEndPoint ipEndPoint, TService service)
        {
            base.Init(id, service, ChannelType.Connect);

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;
            this.innArgs = new SocketAsyncEventArgs();
            this.outArgs = new SocketAsyncEventArgs();
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;

            this.RemoteAddress = ipEndPoint;
            parser.Init(recvBuffer);
            this.isConnected = false;
            this.isSending = false;
        }

        public void Init(long id, Socket socket, TService service)
        {
            base.Init(id, service, ChannelType.Accept);

            this.socket = socket;
            this.socket.NoDelay = true;
            this.innArgs = new SocketAsyncEventArgs();
            this.outArgs = new SocketAsyncEventArgs();
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;

            this.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;
            parser.Init(recvBuffer);
            this.isConnected = true;
            this.isSending = false;
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.socket.Close();
            this.innArgs.Dispose();
            this.outArgs.Dispose();
            this.innArgs = null;
            this.outArgs = null;
            this.socket = null;
            recvBuffer.Dispose();
            sendBuffer.Dispose();
            parser.Dispose();
        }
        public override void Start()
        {
            if (!this.isConnected)
            {
                this.ConnectAsync(this.RemoteAddress);
                return;
            }

            this.StartRecv();
            this.StartSend();
        }
        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    OneThreadSynchronizationContext.Instance.Post(this.OnConnectComplete, e);
                    break;
                case SocketAsyncOperation.Receive:
                    OneThreadSynchronizationContext.Instance.Post(this.OnRecvComplete, e);
                    break;
                case SocketAsyncOperation.Send:
                    OneThreadSynchronizationContext.Instance.Post(this.OnSendComplete, e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    OneThreadSynchronizationContext.Instance.Post(this.OnDisconnectComplete, e);
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        public void ConnectAsync(IPEndPoint ipEndPoint)
        {
            this.outArgs.RemoteEndPoint = ipEndPoint;
            if (this.socket.ConnectAsync(this.outArgs))
            {
                return;
            }
            OnConnectComplete(this.outArgs);
        }

        private void OnConnectComplete(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                OnConnect(false);
                this.OnError((int)e.SocketError);
                return;
            }

            e.RemoteEndPoint = null;
            this.isConnected = true;
            OnConnect(true);
            this.StartRecv();
            this.StartSend();
        }

        private void OnDisconnectComplete(object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            this.OnError((int)e.SocketError);
        }

        private void StartRecv()
        {
            int size = Buffer.ChunkSize - this.recvBuffer.LastIndex;
            this.RecvAsync(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
        }

        public void RecvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this.innArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }

            if (this.socket.ReceiveAsync(this.innArgs))
            {
                return;
            }
            OnRecvComplete(this.innArgs);
        }

        private void OnRecvComplete(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                this.OnError((int)e.SocketError);
                return;
            }

            this.recvBuffer.LastIndex += e.BytesTransferred;
            if (this.recvBuffer.LastIndex == Buffer.ChunkSize)
            {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                Packet packet = this.parser.Parse();
                if (packet == null)
                    break;
                try
                {
                    this.OnRead(packet);
                }
                catch (Exception exception)
                {
                    Log.Error(exception.ToString());
                }
                finally
                {
                    this.parser.Start();
                }
            }

            if (this.socket == null)
            {
                return;
            }

            this.StartRecv();
        }

        private void StartSend()
        {
            if (!this.isConnected)
            {
                return;
            }
            // 没有数据需要发送
            if (this.sendBuffer.Length == 0)
            {
                this.isSending = false;
                return;
            }
            this.isSending = true;

            int sendSize = Buffer.ChunkSize - this.sendBuffer.FirstIndex;
            if (sendSize > this.sendBuffer.Length)
            {
                sendSize = this.sendBuffer.Length;
            }

            this.SendAsync(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
        }

        public void SendAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this.outArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
            if (this.socket.SendAsync(this.outArgs))
            {
                return;
            }
            OnSendComplete(this.outArgs);
        }

        private void OnSendComplete(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }
            this.sendBuffer.FirstIndex += e.BytesTransferred;
            if (this.sendBuffer.FirstIndex == Buffer.ChunkSize)
            {
                this.sendBuffer.FirstIndex = 0;
                this.sendBuffer.RemoveFirst();
            }

            this.StartSend();
        }

		public override void Send(byte[] buffer)
		{
			if (this.Id == 0)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
            cache.WriteTo(0, buffer.Length);
            this.sendBuffer.Write(cache);
            this.sendBuffer.Write(buffer);
			if (this.isConnected)
			{
				this.StartSend();
			}
		}
        public override void Send(byte[] buffer, int dataLength)
        {
            if (this.Id == 0)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            cache.WriteTo(0, dataLength);
            this.sendBuffer.Write(cache);
            this.sendBuffer.Write(buffer, dataLength);
            if (this.isConnected)
            {
                this.StartSend();
            }
        }
        public override void Send(List<byte[]> buffers)
		{
			if (this.Id == 0)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
            int size = buffers.Select(b => b.Length).Sum();

            cache.WriteTo(0, size);
            this.sendBuffer.Write(cache);
            foreach (byte[] buffer in buffers)
            {
                this.sendBuffer.Write(buffer);
            }

            if (this.isConnected)
            {
                this.StartSend();
            }
        }
        public override void Send(MemoryStream stream)
        {
            if (this.Id == 0)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            cache.WriteTo(0, (int)stream.Length);
            this.sendBuffer.Write(cache);
            stream.Position = 0;
            this.sendBuffer.Write(stream);
            if (this.isConnected)
            {
                this.StartSend();
            }
        }

	}
}