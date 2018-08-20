using Base;
using Data;
using System.Collections.Generic;
using System.IO;

namespace EventHandle
{
    [Event((int)Base.InnerEventIdType.SessionTranspond, AppType.GateServer)]
    public class SessionTranspondEventHandle : IEvent<Session, ProtocolInfo, List<long>, MemoryStream>
    {
        public void Run(Session session, ProtocolInfo protocolInfo, List<long> toIds, MemoryStream stream)
        {
            if (protocolInfo.ToServer == AppType.Client)
            {//转发客户端
                foreach(long id in toIds)
                    TranspondComponent.instance.ToClient(id, stream);
            }
            else if (protocolInfo.FromServer == AppType.Client)
            {//来自客户端,转发其他服务器
                ErrorCode isSuccess = TranspondComponent.instance.ToServer(protocolInfo.ToServer, toIds[0], stream);
                if (isSuccess == ErrorCode.Fail)
                {
                    session.Dispose();
                }
            }
        }
    }
}
