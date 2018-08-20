using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_PlayerEnterGameRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int gateAppid{set;get;}

        public void Dispose()
        {
            id = 0;
            gateAppid = 0;
        }
    }
}
