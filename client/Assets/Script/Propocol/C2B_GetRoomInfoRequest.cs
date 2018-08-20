
using System;


namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2B_GetRoomInfoRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long roomid { get; set; }

       

        public void Dispose()
		{
            roomid = 0;
        }
	}
}
