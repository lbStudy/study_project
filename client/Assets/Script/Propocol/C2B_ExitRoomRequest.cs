using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2B_ExitRoomRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public long roomid{set;get;}

        public void Dispose()
        {
            roomid = 0;
        }
    }
}
