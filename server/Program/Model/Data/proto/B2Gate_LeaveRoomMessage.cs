using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2Gate_LeaveRoomMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public long roomid{set;get;}

        public void Dispose()
        {
            roomid = 0;
        }
    }
}
