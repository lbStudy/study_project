using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2B_PingBackMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long curClienTime { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public long roomid { get; set; }
        public void Dispose()
		{
            curClienTime = 0;
            roomid = 0;
        }
	}
}
