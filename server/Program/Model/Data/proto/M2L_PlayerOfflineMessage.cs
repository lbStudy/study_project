using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2L_PlayerOfflineMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        public void Dispose()
        {
            id = 0;
        }
    }
}
