using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_PlayerConnectGateNotifyRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        public void Dispose()
        {
            id = 0;
        }
    }
}
