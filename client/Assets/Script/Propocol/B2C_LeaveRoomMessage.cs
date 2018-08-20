using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_LeaveRoomMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }


        public void Dispose()
        {
            id = 0;
        }
    }
}
