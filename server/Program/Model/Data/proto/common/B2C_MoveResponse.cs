using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_MoveResponse : IDisposable
	{
		[ProtoBuf.ProtoMember(1)]
public int errorCode { get; set; }
		public void Dispose()
		{
		    errorCode = 0;
		}
	}
}
