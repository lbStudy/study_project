using System;
using System.Collections.Generic;

namespace Base
{
    public interface IProtocolHandle
    {
        void Handle(Session session, ProtocolInfo protoInfo, object msg, List<long> toids, uint rpcId = 0);
    }
    /// <summary>
    /// 客户端或者服务端非RPC消息处理
    /// </summary>
    /// <typeparam name="Message"></typeparam>
    public abstract class AMHandler<T> : IProtocolHandle where T : class
    {
        protected abstract void Run(MsgPacakage package);

        public void Handle(Session session, ProtocolInfo protoInfo, object msg, List<long> toids, uint rpcId = 0)
        {
            MsgPacakage package = ObjectPoolManager.Instance.Take<MsgPacakage>();
            package.Init(session, protoInfo, msg, toids);
            T message = msg as T;
            if (message == null)
            {
                package.Dispose();
                Log.Debug($"消息类型转换错误: {msg.GetType().Name} to {typeof(T).Name}.");
                return;
            }
            this.Run(package);
        } 
    }
    /// <summary>
    /// 服务端RPC请求处理
    /// </summary>
    /// <typeparam name="Request"></typeparam>
    /// <typeparam name="Response"></typeparam>
	public abstract class AMRpcHandler<TRequest> : IProtocolHandle where TRequest : class
    {
        protected abstract void Run(RpcPacakage package);

        public void Handle(Session session, ProtocolInfo protoInfo, object msg, List<long> toids, uint rpcId = 0)
        {
            RpcPacakage package = ObjectPoolManager.Instance.Take<RpcPacakage>();

            ProtocolInfo respProtoInfo = ProtocolDispatcher.Instance.GetProtocolInfo(protoInfo.Opcode + 100000);
            package.Init(session, protoInfo, respProtoInfo, msg, rpcId, toids);

            TRequest request = msg as TRequest;
            if (request == null)
            {
                package.Dispose();
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(TRequest).Name}");
                return;
            }

            this.Run(package);
        }
    }
}
