using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2L_ReloginMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int areaid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string name{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int sex{set;get;}

        public void Dispose()
        {
            id = 0;
            areaid = 0;
            name = string.Empty;
            iconUrl = string.Empty;
            sex = 0;
        }
    }
}
