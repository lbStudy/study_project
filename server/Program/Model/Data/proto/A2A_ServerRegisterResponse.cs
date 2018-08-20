using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class A2A_ServerRegisterResponse : IDisposable
	{
		[ProtoBuf.ProtoMember(1)]
public int errorCode { get; set; }
		public void Dispose()
		{
		    errorCode = 0;
		}
	}
}
