using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2B_PlayerOfflineMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public long roomid{set;get;}

        public void Dispose()
        {
            id = 0;
            roomid = 0;
        }
    }
}
