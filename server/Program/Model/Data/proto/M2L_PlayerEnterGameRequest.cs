using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2L_PlayerEnterGameRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int bigAreaid{set;get;}

        public void Dispose()
        {
            id = 0;
            bigAreaid = 0;
        }
    }
}
