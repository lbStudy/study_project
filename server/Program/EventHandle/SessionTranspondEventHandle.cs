using Base;
using Data;
using System.Collections.Generic;
using System.IO;

namespace EventHandle
{
    [Event((int)Base.InnerEventIdType.SessionTranspond, AppType.GateServer)]
    public class SessionTranspondEventHandle : IEvent<Session, ProtocolInfo, List<long>, MemoryStream>
    {
        public static byte[] countByte = new byte[2];
        public void Run(Session session, ProtocolInfo protocolInfo, List<long> toIds, MemoryStream stream)
        {
            if (protocolInfo.ToServer == AppType.Client)
            {//转发客户端
                Packet packet = null;
                try
                {
                    if (toIds.Count > 1)
                    {
                        int offset = 2 + toIds.Count * 8;
                        int length = (int)stream.Length - offset;
                        packet = Packet.Take(length + countByte.Length);
                        packet.Stream.Write(countByte, offset, countByte.Length);
                        packet.Stream.Write(stream.GetBuffer(), offset, length);
                        stream = packet.Stream;
                    }
                    foreach (long id in toIds)
                    {
                        Session clientSession = NetOuterComponent.Instance.FindByRelevanceID(id);
                        clientSession.SendMessage(stream);
                    }
                }
                finally
                {
                    if (packet != null)
                    {
                        Packet.Back(packet);
                    }
                }
            }
            else if (protocolInfo.FromServer == AppType.Client)
            {//来自客户端,转发其他服务器
                //net.ge
                ErrorCode isSuccess = TranspondComponent.instance.ToServer(protocolInfo.ToServer, toIds[0], stream);
                if (isSuccess == ErrorCode.Fail)
                {
                    session.Dispose();
                }
            }
        }
    }
}
