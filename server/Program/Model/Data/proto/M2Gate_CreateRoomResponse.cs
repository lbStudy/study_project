using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_CreateRoomResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(1)]
        public long id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int battleAppid{set;get;}

        public void Dispose()
        {
            id = 0;
            errorCode = 0;
            battleAppid = 0;
        }
    }
}
