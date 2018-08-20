using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2Gate_EnterRoomResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }

        public void Dispose()
        {
            errorCode = 0;
        }
    }
}
