using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_EnterRoomResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int battleAppid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public long roomid { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public int sceneid { set; get; }
        public void Dispose()
        {
            errorCode = 0;
            battleAppid = 0;
            roomid = 0;
        }
    }
}
