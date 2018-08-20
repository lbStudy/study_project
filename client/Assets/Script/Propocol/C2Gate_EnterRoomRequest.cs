using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2Gate_EnterRoomRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public int roomid { get; set; }
        public void Dispose()
		{
            roomid = 0;
        }
	}
}
