using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace Base
{
    [Pool]
    public sealed class Session : Disposer, Initer<long, NetworkComponent, AChannel>
    {
        private long id;
        public long Id { get { return id; } }
        private static uint RpcId { get; set; }
        private NetworkComponent network;
        private Dictionary<uint, TaskCompletionSource<object>> requestCallback = new Dictionary<uint, TaskCompletionSource<object>>();
        private AChannel channel;
        public bool IsConnect { get { return channel.IsConnected; } }
        public long relevanceID;
        public long pingTime;
        byte[] idByte = new byte[8];
        byte[] rpcByte = new byte[4];
        public List<long> ids = new List<long>();
        public void Init(long id, NetworkComponent network, AChannel channel)
        {
            this.id = id;
            this.network = network;
            this.channel = channel;
            this.relevanceID = 0;

            this.channel = channel;
            this.requestCallback.Clear();
            channel.ReadCallback += this.OnRead;

            this.channel.Start();
        }
        public IPEndPoint RemoteAddress
        {
            get
            {
                return this.channel.RemoteAddress;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return this.channel.ChannelType;
            }
        }
        private void OnRead(Packet packet)
        {
            //byte[] messageBytes, int dataLength
            ids.Clear();
            byte[] messageBytes = packet.Stream.GetBuffer();
            int offset = 0;
            short count = BitConverter.ToInt16(messageBytes, offset); ;
            offset += 2;
            for (int i = 0; i < count; i++)
            {
                long id = BitConverter.ToInt64(messageBytes, offset);
                ids.Add(id);
                offset += 8;
            }
            int opcode = BitConverter.ToInt32(messageBytes, offset);
            offset += 4;
            byte selects = messageBytes[offset];
            bool isEncrypt = (selects & 2) > 0;
            offset += 1;
            uint rpcId = BitConverter.ToUInt32(messageBytes, offset);
            offset += 4;

            ProtocolInfo protocolInfo = ProtocolDispatcher.Instance.GetProtocolInfo(opcode);

            if (protocolInfo == null)
            {//接受到的消息码不存在，断开连接
                Dispose();
                Log.Debug($"session({relevanceID}) receive not exist opcode({opcode}) message ");
                return;
            }

            if (protocolInfo.ProtocolCategory != ProtocolCategory.Message && rpcId <= 0)
            {//接受到的RPC消息，但是Rpcid为<=0，断开连接
                Dispose();
                Log.Debug($"session({relevanceID}) receive rpc message({opcode}), but rpcid <= 0.");
                return;
            }

            if (protocolInfo.ToServer == network.AppType || protocolInfo.ToServer == AppType.All)
            {//当前服务器处理
                if (isEncrypt)
                {

                }

                object message = null;

                if (protocolInfo.ProtocolCategory == ProtocolCategory.Response)
                {
                    TaskCompletionSource<object> tcs;
                    if (this.requestCallback.TryGetValue(rpcId, out tcs))
                    {
                        try
                        {
                            message = tcs.Task.AsyncState;
                            packet.Stream.Position = offset;
                            message = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(packet.Stream, message, protocolInfo.ProtocolBodyType);
                        }
                        catch
                        {
                            Dispose();
                            Log.Warning("analysis error.");
                            return;
                        }
                        this.requestCallback.Remove(rpcId);
                        tcs.SetResult(message);
                    }
                }
                else
                {
                    if (protocolInfo.HandleInterface == null)
                    {
                        Log.Warning($"not find protocol handle func, opcode({protocolInfo.Opcode})");
                        return;
                    }
                    try
                    {
                        message = protocolInfo.Take();
                        packet.Stream.Position = offset;
                        message = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(packet.Stream, message, protocolInfo.ProtocolBodyType);
                    }
                    catch
                    {
                        Dispose();
                        Log.Warning("analysis error.");
                        return;
                    }

                    if(protocolInfo.FromServer == AppType.Client 
                        && network.IsOuter)
                    {
                        if(count != 1)
                        {
                            Dispose();
                            Log.Warning("If proto from client, should not exist toid count != 1.");
                            return;
                        }
                        ids[0] = relevanceID;
                    }
                    protocolInfo.HandleInterface.Handle(this, protocolInfo, message, ids, rpcId);
                }
            }
            else
            {//转发
                if(protocolInfo.FromServer == AppType.Client)
                {//前端消息进过了网关验证才能转发到内外
                    if(relevanceID == 0)
                    {
                        Dispose();
                        return;
                    }
                    if(network.IsOuter)
                    {
                        if (count != 1)
                        {
                            Dispose();
                            Log.Warning("If proto from client, should not exist toid count != 1.");
                            return;
                        }
                        ids[0] = relevanceID;
                        idByte.WriteTo(0, relevanceID);
                        Array.Copy(idByte, 0, messageBytes, 2, 8);
                    }
                }
                EventDispatcher.Instance.Run((int)InnerEventIdType.SessionTranspond, this, protocolInfo, ids, packet.Stream);
            }
            ids.Clear();
        }
        /// <summary>
        /// Rpc调用,发送一个消息,等待返回一个消息
        /// </summary>
        public Task<object> Call(object request, long sendId, object response = null)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            TaskCompletionSource<object> tcs = null;
            Packet packet = Packet.Take();
            try
            {
                ids.Clear();
                ids.Add(sendId);
                FillContent(packet.Stream, request, ++RpcId, ids);
                this.SendMessage(packet.Stream);
                tcs = new TaskCompletionSource<object>(response);
                this.requestCallback[RpcId] = tcs;
            }
            finally
            {
                Packet.Back(packet);
            }
            return tcs.Task;
        }
        public void SendMessage(object message, long sendId)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                ids.Clear();
                ids.Add(sendId);
                FillContent(packet.Stream, message, 0, ids);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public void SendMessage(object message, List<long> sendIds)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                FillContent(packet.Stream, message, 0, sendIds);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public void Reply(uint rpcId, object message, long sendId, int errorCode = 0)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                ids.Clear();
                ids.Add(sendId);
                FillContent(packet.Stream, message, rpcId, ids);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public void FillContent(MemoryStream stream, object message, uint rpcId, List<long> sendIds)
        {
            if (stream == null)
            {
                throw new Exception($"stream is null.");
            }
            ProtocolInfo protocolInfo = ProtocolDispatcher.Instance.GetProtocolInfo(message.GetType());

            if (protocolInfo == null)
            {
                throw new Exception($"Not exist {message.GetType().Name} protocol.");
            }

            short count = sendIds == null ? (short)0 :(short)sendIds.Count;
            idByte.WriteTo(0, count);
            stream.Write(idByte, 0, 2);
            if (sendIds != null && sendIds.Count > 0)
            {
                foreach (long id in sendIds)
                {
                    idByte.WriteTo(0, id);
                    stream.Write(idByte, 0, idByte.Length);
                }
            }
            stream.Write(protocolInfo.opcodeBytes, 0, protocolInfo.opcodeBytes.Length);
            stream.Write(protocolInfo.selectsBytes, 0, protocolInfo.selectsBytes.Length);
            rpcByte.WriteTo(0, rpcId);
            stream.Write(rpcByte, 0, rpcByte.Length);
            ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(stream, message);
        }
        public void SendMessage(MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                Console.WriteLine($"session {id}已经被Dispose了.");
                return;
            }
            if (stream == null)
            {
                throw new Exception($"stream is null.");
            }
            channel.Send(stream);
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.channel.Dispose();
            this.network.Remove(this);
            this.channel = null;
            this.network = null;
            if (this.requestCallback.Count > 0)
            {
                this.requestCallback.Clear();
                Log.Warning("session rpc request not handle, but session is removed.");
            }
            relevanceID = 0;
            ids.Clear();
        }
    }
}
