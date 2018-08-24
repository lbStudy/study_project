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

        public long relevanceID;
        public long pingTime;
        byte[] idByte = new byte[8];
        byte[] rpcByte = new byte[4];
        public List<long> toids = new List<long>();
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
            byte[] messageBytes = packet.Stream.GetBuffer();
            int offset = 0;
            ushort count = BitConverter.ToUInt16(messageBytes, offset); ;
            offset += 2;
            for (int i = 0; i < count; i++)
            {
                long id = BitConverter.ToInt64(messageBytes, offset);
                if(id > 0)
                    toids.Add(id);
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
                        && network is NetOuterComponent)
                    {
                        if(toids.Count > 0)
                        {
                            Dispose();
                            Log.Warning("If proto from client, should not exist toid count > 0.");
                            return;
                        }
                        toids.Add(relevanceID);
                    }
                    protocolInfo.HandleInterface.Handle(this, protocolInfo, message, toids, rpcId);
                }
            }
            else
            {//转发
                if(protocolInfo.FromServer == AppType.Client)
                {
                    if(relevanceID == 0)
                    {
                        Dispose();
                        return;
                    }
                    if(network is NetOuterComponent)
                    {
                        if (toids.Count > 0)
                        {
                            Dispose();
                            Log.Warning("If proto from client, should not exist toid count > 0.");
                            return;
                        }
                        toids.Add(relevanceID);
                        count = 1;
                        idByte.WriteTo(0, count);
                        Array.Copy(idByte, 0, messageBytes, 0, 2);
                        idByte.WriteTo(0, relevanceID);
                        Array.Copy(idByte, 0, messageBytes, 2, 8);
                    }
                }
                EventDispatcher.Instance.Run((int)InnerEventIdType.SessionTranspond, this, protocolInfo, toids, packet.Stream);
            }
            toids.Clear();
        }
        /// <summary>
        /// Rpc调用,发送一个消息,等待返回一个消息
        /// </summary>
        public Task<object> Call(object request, long toId, object response = null)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            TaskCompletionSource<object> tcs = null;
            Packet packet = Packet.Take();
            try
            {
                FillContent(packet.Stream, request, ++RpcId);
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
        public void SendMessage(object message, long toid)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                toids.Clear();
                toids.Add(toid);
                FillContent(packet.Stream, message, 0);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public void SendMessage(object message, List<long> toids)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                if(this.toids != toids)
                {
                    this.toids.Clear();
                    this.toids.AddRange(toids);
                }
                FillContent(packet.Stream, message, 0);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public void Reply(uint rpcId, object message, long toid, int errorCode = 0)
        {
            if (this.IsDisposed)
            {
                throw new Exception($"session已经被Dispose了.");
            }
            Packet packet = Packet.Take();
            try
            {
                FillContent(packet.Stream, message, rpcId, toid);
                this.SendMessage(packet.Stream);
            }
            finally
            {
                Packet.Back(packet);
            }
        }
        public static void ReplaceToid(MemoryStream stream, long toid)
        {
            if (stream == null)
            {
                throw new Exception($"stream is null.");
            }
            stream.Position = 0;
            byte[] idBytes = BitConverter.GetBytes(toid);
            stream.Write(idBytes, 0, idBytes.Length);
        }
        public void FillContent(MemoryStream stream, object message, uint rpcId)
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

            if (toids.Count > 0)
            {
                short count = (short)toids.Count;
                idByte.WriteTo(0, count);
                stream.Write(idByte, 0, 2);
                foreach (long id in toids)
                {
                    idByte.WriteTo(0, id);
                    stream.Write(idByte, 0, idByte.Length);
                }
            }
            //else
            //{
            //    short count = (short)(toServer ? -1 : 1);
            //    idByte.WriteTo(0, count);
            //    stream.Write(idByte, 0, 2);
            //    idByte.WriteTo(0, 0L);
            //    stream.Write(idByte, 0, idByte.Length);
            //}
            stream.Write(protocolInfo.opcodeBytes, 0, protocolInfo.opcodeBytes.Length);
            stream.Write(protocolInfo.selectsBytes, 0, protocolInfo.selectsBytes.Length);
            rpcByte.WriteTo(0, rpcId);
            stream.Write(rpcByte, 0, rpcByte.Length);
            ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(stream, message);
        }
        public static void FillContent(MemoryStream stream, object message, uint rpcId, long toid)
        {
            if(stream == null)
            {
                throw new Exception($"stream is null.");
            }
            ProtocolInfo protocolInfo = ProtocolDispatcher.Instance.GetProtocolInfo(message.GetType());

            if (protocolInfo == null)
            {
                throw new Exception($"Not exist {message.GetType().Name} protocol.");
            }
            byte[] idBytes = BitConverter.GetBytes(toid);
            stream.Write(idBytes, 0, idBytes.Length);
            stream.Write(protocolInfo.opcodeBytes, 0, protocolInfo.opcodeBytes.Length);
            stream.Write(protocolInfo.selectsBytes, 0, protocolInfo.selectsBytes.Length);
            byte[] seqBytes = BitConverter.GetBytes(rpcId);
            stream.Write(seqBytes, 0, seqBytes.Length);
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
            toids.Clear();
        }
    }
}
