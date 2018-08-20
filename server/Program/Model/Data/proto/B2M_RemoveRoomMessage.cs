using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2M_RemoveRoomMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int battleAppid{set;get;}

        public void Dispose()
        {
            id = 0;
            battleAppid = 0;
        }
    }
}
