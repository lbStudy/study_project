using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_GmCommonOpRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int op{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public int param1{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public int param2{set;get;}

        public void Dispose()
        {
            id = 0;
            op = 0;
            param1 = 0;
            param2 = 0;
        }
    }
}
