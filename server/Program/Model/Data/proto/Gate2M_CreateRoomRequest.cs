using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_CreateRoomRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(3)]
        public int totalCount { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int jushu_option { get; set; }

        public void Dispose()
        {
            totalCount = 0;
            jushu_option = 0;
        }
    }
}