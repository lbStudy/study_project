using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Base;
using UnityEngine;

namespace Model
{
	public class TChannel : AChannel
	{
		private readonly TcpClient tcpClient;

		private readonly TBuffer recvBuffer = new TBuffer();
		private readonly TBuffer sendBuffer = new TBuffer();
        private bool isSending;
        private readonly PacketParser parser;
		private TaskCompletionSource<byte[]> recvTcs;
        //public IAsyncResult recvResult;
        //public IAsyncResult sendResult;
        /// <summary>
        /// connect
        /// </summary>
        System.Action<string, SocketError> mCallBack = null;
		public TChannel(TcpClient tcpClient, string host, int port, TService service, System.Action<string, SocketError> callback) : base(service, ChannelType.Connect)
		{
			this.tcpClient = tcpClient;
			this.parser = new PacketParser(this.recvBuffer);
			this.RemoteAddress = host + ":" + port;
            mCallBack = callback;

            this.ConnectAsync(host, port);
		}

		/// <summary>
		/// accept
		/// </summary>
		public TChannel(TcpClient tcpClient, TService service) : base(service, ChannelType.Accept)
		{
			this.tcpClient = tcpClient;
			this.parser = new PacketParser(this.recvBuffer);

			IPEndPoint ipEndPoint = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
			this.RemoteAddress = ipEndPoint.Address + ":" + ipEndPoint.Port;
			//this.OnAccepted();
		}
        public void Update()
        {
            //if (Connected)
            //{
            //    try
            //    {
            //        if (recvResult == null)
            //        {
            //            NetworkStream stream = this.tcpClient.GetStream();
            //            if (stream.CanRead && stream.DataAvailable)
            //            {
            //                int size = TBuffer.ChunkSize - this.recvBuffer.LastIndex;
            //                recvResult = stream.BeginRead(this.recvBuffer.Last, this.recvBuffer.LastIndex, size, null, null);
            //            }
            //        }
            //        else if (recvResult.IsCompleted)
            //        {
            //            NetworkStream stream = this.tcpClient.GetStream();
            //            int NumOfBytesRead = stream.EndRead(recvResult);
            //            if (NumOfBytesRead > 0)
            //            {
            //                this.recvBuffer.LastIndex += NumOfBytesRead;
            //                if (this.recvBuffer.LastIndex == TBuffer.ChunkSize)
            //                {
            //                    this.recvBuffer.AddLast();
            //                    this.recvBuffer.LastIndex = 0;
            //                }
            //                if (this.recvTcs != null)
            //                {
            //                    byte[] packet = this.parser.GetPacket();
            //                    if (packet != null)
            //                    {
            //                        var tcs = this.recvTcs;
            //                        this.recvTcs = null;
            //                        tcs.SetResult(packet);
            //                    }
            //                }
            //                recvResult = null;
            //            }
            //        }
            //        if (null == sendResult)
            //        {
            //            StartSend();
            //        }
            //        if (sendResult != null)
            //        {
            //            if (sendResult.IsCompleted)
            //            {

            //                tcpClient.GetStream().EndWrite(sendResult);
            //                sendResult = null;
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        UnityEngine.Debug.Log(e.ToString());
            //        this.OnError(this, SocketError.SocketError);
            //    }
            //}
            //else if (sendResult != null)
            //{
            //    this.OnError(this, SocketError.SocketError);
            //}
        }
        private async void ConnectAsync(string host, int port)
		{
            //recvResult = null;
            //sendResult = null;
            try
            {
                await tcpClient.ConnectAsync(host, port);
                tcpClient.SendTimeout = 2;
                tcpClient.ReceiveTimeout = 2;
                if (null != mCallBack)
                    mCallBack(null, SocketError.Success);
                this.StartSend();
                this.StartRecv();
            }
            catch(System.Exception e)
            {
                this.OnError(this, SocketError.SocketError);
                if (null != mCallBack)
                    mCallBack(e.Message, SocketError.SocketError);
            }
          
        }

		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}
			
			base.Dispose();
            //if(null != sendResult)
            //{

            //}
            //sendResult = null;
            //recvResult = null;
			this.tcpClient.Close();

        }
        public override bool Connected
        {
            get
            {
                return null != tcpClient &&  tcpClient.Connected;
            }
        }
  //      private void OnAccepted()
		//{
		//	this.StartSend();
		//	this.StartRecv();
		//}

		public override void Send(byte[] buffer, byte channelID = 0, PacketFlags flags = PacketFlags.Reliable)
		{
			if (this.Id == 0)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
			byte[] size = BitConverter.GetBytes((ushort)buffer.Length);
			this.sendBuffer.SendTo(size);
			this.sendBuffer.SendTo(buffer);
			if (Connected)
			{
				this.StartSend();
			}
            else if (this.Id != 0)
            {
                this.OnError(this, SocketError.SocketError);
            }
        }

		public override void Send(List<byte[]> buffers, byte channelID = 0, PacketFlags flags = PacketFlags.Reliable)
		{
			if (this.Id == 0)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
			int size = buffers.Select(b => b.Length).Sum();
			byte[] sizeBuffer = BitConverter.GetBytes(size);
			this.sendBuffer.SendTo(sizeBuffer);
			foreach (byte[] buffer in buffers)
			{
				this.sendBuffer.SendTo(buffer);
			}
            if (Connected)
            {
                this.StartSend();
            }
            else if(this.Id != 0)
            {
                this.OnError(this, SocketError.SocketError);
            }
        }

        private async void StartSend()
        {
            try
            {
                if (this.Id == 0)
                {
                    return;
                }

                // 如果正在发送中,不需要再次发送
                if (this.isSending)
                {
                    return;
                }

                while (true)
                {
                    if (this.Id == 0)
                    {
                        return;
                    }

                    // 没有数据需要发送
                    if (this.sendBuffer.Count == 0)
                    {
                        this.isSending = false;
                        return;
                    }

                    this.isSending = true;

                    int sendSize = TBuffer.ChunkSize - this.sendBuffer.FirstIndex;
                    if (sendSize > this.sendBuffer.Count)
                    {
                        sendSize = this.sendBuffer.Count;
                    }
                    await this.tcpClient.GetStream().WriteAsync(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);
                    this.sendBuffer.FirstIndex += sendSize;
                    if (this.sendBuffer.FirstIndex == TBuffer.ChunkSize)
                    {
                        this.sendBuffer.FirstIndex = 0;
                        this.sendBuffer.RemoveFirst();
                    }
                }
            }
            catch (Exception e)
            {

                Debug.Log(e.ToString());
                this.OnError(this, SocketError.SocketError);
            }
        }
        private async void StartRecv()
        {
            try
            {
                while (true)
                {
                    if (this.Id == 0)
                    {
                        return;
                    }
                    int size = TBuffer.ChunkSize - this.recvBuffer.LastIndex;

                    int n = await this.tcpClient.GetStream().ReadAsync(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);

                    if (n == 0)
                    {
                        this.OnError(this, SocketError.NetworkReset);
                        return;
                    }

                    this.recvBuffer.LastIndex += n;

                    if (this.recvBuffer.LastIndex == TBuffer.ChunkSize)
                    {
                        this.recvBuffer.AddLast();
                        this.recvBuffer.LastIndex = 0;
                    }

                    if (this.recvTcs != null)
                    {
                        byte[] packet = this.parser.GetPacket();
                        if (packet != null)
                        {
                            var tcs = this.recvTcs;
                            this.recvTcs = null;
                            tcs.SetResult(packet);
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                this.OnError(this, SocketError.SocketError);
            }
            catch (IOException)
            {
                this.OnError(this, SocketError.SocketError);
            }
            catch (Exception e)
            {
               
                this.OnError(this, SocketError.SocketError);
            }
        }

        public override Task<byte[]> Recv()
		{
			if (this.Id == 0)
			{
				throw new Exception("TChannel已经被Dispose, 不能接收消息");
			}

			byte[] packet = this.parser.GetPacket();
			if (packet != null)
			{
				return Task.FromResult(packet);
			}

			recvTcs = new TaskCompletionSource<byte[]>();
			return recvTcs.Task;
		}
	}
}