using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_ReloginMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int gateAppid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string name{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int sex{set;get;}

        public void Dispose()
        {
            id = 0;
            gateAppid = 0;
            name = string.Empty;
            iconUrl = string.Empty;
            sex = 0;
        }
    }
}
