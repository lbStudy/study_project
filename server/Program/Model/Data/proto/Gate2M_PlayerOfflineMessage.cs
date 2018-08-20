using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_PlayerOfflineMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        public void Dispose()
        {
            id = 0;
        }
    }
}
