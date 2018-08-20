using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_EnterRoomRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(1)]
        public long id { get; set; }
        public void Dispose()
        {
            id = 0;
        }
    }
}
