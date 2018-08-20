using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2B_LeaveRoomRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long playerid { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public long roomid { get; set; }
        public void Dispose()
		{
            playerid = 0;
            roomid = 0;
		}
	}
}
