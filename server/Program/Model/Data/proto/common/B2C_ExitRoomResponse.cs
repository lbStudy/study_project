using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_ExitRoomResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }

        public void Dispose()
        {
            errorCode = 0;
        }
    }
}
