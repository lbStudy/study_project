using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class L2M_PlayerEnterGameResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public string name{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public int sex{set;get;}
        public void Dispose()
        {
            errorCode = 0;
            name = string.Empty;
            iconUrl = string.Empty;
            sex = 0;
        }
    }
}
