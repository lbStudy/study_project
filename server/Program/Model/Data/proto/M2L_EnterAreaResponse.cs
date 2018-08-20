using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2L_EnterAreaResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public string gateip{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public int gateport{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public string chatip{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int chatport{set;get;}
        [ProtoBuf.ProtoMember(7)]
        public string checkcode{set;get;}

        public void Dispose()
        {
            errorCode = 0;
            gateip = string.Empty;
            gateport = 0;
            chatip = string.Empty;
            chatport = 0;
            checkcode = string.Empty;
        }
    }
}
