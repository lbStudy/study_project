using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Model;

namespace Base
{
    public sealed class Session : Entity
    {
        private static uint RpcId { get; set; }
        private readonly NetworkComponent network;
        private readonly Dictionary<uint, TaskCompletionSource<object>> requestCallback = new Dictionary<uint, TaskCompletionSource<object>>();
        public readonly AChannel channel;

        public long relevanceID;
        public bool Connected
        {
            get
            {
                if (null != channel)
                    return channel.Connected;
                else
                    return false;
            }
        }

        public Session(NetworkComponent network, AChannel channel) : base(EntityType.Session)
        {
            this.network = network;
            this.channel = channel;
            this.relevanceID = 0;
            this.StartRecv();
        }
        public string RemoteAddress
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
        public void Update()
        {
            TChannel tc = channel as TChannel;
            if(tc != null)
            {
                tc.Update();
            }
        }
        private async void StartRecv()
        {
            while (true)
            {
                if (this.IsDisposer)
                {
                    return;
                }

                byte[] messageBytes;
                try
                {
                    messageBytes = await channel.Recv();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.ToString());
                    continue;
                }

                if (messageBytes.Length < 9)
                {
                    continue;
                }

                try
                {
                    this.Run(messageBytes);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.ToString());
                }
            }
        }

        private void Run(byte[] messageBytes)
        {
            int offset = 0;
            long playerid = BitConverter.ToInt64(messageBytes, offset);
            offset += 8;
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
                //Dispose();
                UnityEngine.Debug.LogError($"session({relevanceID}) receive not exist opcode({opcode}) message ");
                return;
            }

            if(protocolInfo.ProtocolCategory != ProtocolCategory.Message && rpcId <= 0)
            {//接受到的RPC消息，但是Rpcid为<=0，断开连接
                //Dispose();
                UnityEngine.Debug.LogError($"session({relevanceID}) receive rpc message({opcode}), but rpcid <= 0.");
                return;
            }

            if (isEncrypt)
            {

            }
            //if (isCompressed)
            //{
            //    messageBytes = ZipHelper.Decompress(messageBytes, offset, messageBytes.Length - offset);
            //    offset = 0;
            //}

            object message = protocolInfo.Take();
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(messageBytes, offset, messageBytes.Length - offset))
                {
                    message = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, message, protocolInfo.ProtocolBodyType);
                }
            }
            catch(System.Exception e)
            {
                //Dispose();
                protocolInfo.Back(message);
                UnityEngine.Debug.LogError(e.Message);
                UnityEngine.Debug.LogError(e.StackTrace);
                return;
            }
            if (protocolInfo.ProtocolCategory == ProtocolCategory.Response)
            {
                TaskCompletionSource<object> tcs;
                // Rpc回调有找不着的可能，因为client可能取消Rpc调用
                if (!this.requestCallback.TryGetValue(rpcId, out tcs))
                {
                    return;
                }
                this.requestCallback.Remove(rpcId);
                tcs.SetResult(message);
            }
            else
            {
                if (protocolInfo.HandleInterface == null)
                {
                    UnityEngine.Debug.LogError($"not find protocol handle func, opcode({protocolInfo.Opcode})");
                    return;
                }
                protocolInfo.HandleInterface.Handle(this, message, rpcId);
            }
            protocolInfo.Back(message);
        }
        /// <summary>
        /// Rpc调用
        /// </summary>
        public Task<object> Call(object request, CancellationToken cancellationToken)
        {
            if (this.IsDisposer)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.SendMessage(++RpcId, request);

            var tcs = new TaskCompletionSource<object>();

            this.requestCallback[RpcId] = tcs;

            cancellationToken.Register(() => { this.requestCallback.Remove(RpcId); });

            return tcs.Task;
        }

        /// <summary>
        /// Rpc调用,发送一个消息,等待返回一个消息
        /// </summary>
        public Task<object> Call(object request)
        {
            if (this.IsDisposer)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.SendMessage(++RpcId, request);

            var tcs = new TaskCompletionSource<object>();

            this.requestCallback[RpcId] = tcs;

            //var tcs = new TaskCompletionSource<TResponse>();

            //this.requestCallback[RpcId] = (object response) =>
            //{
            //    tcs.SetResult(response as TResponse);
            //};

            return tcs.Task;
        }
        //推送消息
        public void Send(object message)
        {
            List<byte[]> content = GetSendContent(0, message);
            this.SendMessage(content);
        }
        /// <summary>
        /// 仅用于消息，不可用于请求和响应。适用于消息广播给多个session。
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<byte[]> MessageTransform<TMessage>(TMessage message) where TMessage : class
        {
            return GetSendContent(0, message);
        }
        public void SendMessage(uint rpcId, object msg)
        {
            List<byte[]> content = GetSendContent(rpcId, msg);
            this.SendMessage(content);
        }
        //请求回复
        public void Reply<TResponse>(uint rpcId, TResponse message) where TResponse : class
        {
            List <byte[]> content = GetSendContent(rpcId, message);
            this.SendMessage(content);
        }
        private static List<byte[]> GetSendContent(uint rpcId, object message)
        {
            ProtocolInfo protocolInfo = ProtocolDispatcher.Instance.GetProtocolInfo(message.GetType());

            if (protocolInfo == null)
            {
                UnityEngine.Debug.LogError($"not exist {message.GetType().Name} protocol.");
                return null;
            }
            byte[] opcodeBytes = BitConverter.GetBytes(protocolInfo.Opcode);

            byte[] messageBytes = null;//message.ToBson();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(ms, message);
                messageBytes = ms.ToArray();
            }
            byte[] selects = new byte[1];
            selects[0] = 0;

            if (protocolInfo.IsEncrypt)
            {//加密处理
                selects[0] |= 2;
            }

            byte[] seqBytes = BitConverter.GetBytes(rpcId);
            byte[] idBytes = BitConverter.GetBytes(0L);
            return new List<byte[]> { idBytes, opcodeBytes, selects, seqBytes,  messageBytes };
        }
        private void SendMessage(List<byte[]> content)
        {
            if (this.IsDisposer)
            {
                throw new Exception("session已经被Dispose了");
            }
            if (content == null)
            {
                throw new Exception("session send, content is null.");
            } 
            channel.Send(content);
        }

        public override void Dispose()
        {
            if (this.IsDisposer)
            {
                return;
            }

            base.Dispose();

            this.channel.Dispose();
            this.network.Remove(this);

            if (this.requestCallback.Count > 0)
            {
                UnityEngine.Debug.Log("session rpc request not handle, but session is removed.");
            }
            relevanceID = 0;
        }
    }
}
