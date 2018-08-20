using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_EnterRoomResponse : IDisposable
	{
		[ProtoBuf.ProtoMember(1)]
public int errorCode { get; set; }
		public void Dispose()
		{
		    errorCode = 0;
		}
	}
}
