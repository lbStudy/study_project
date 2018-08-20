using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_RequestOrderResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public int error;
        [ProtoBuf.ProtoMember(4)]
        public string serial { get; set; }

        public void Dispose()
        {
            error = 0;
            serial = string.Empty;
        }
    }
}
