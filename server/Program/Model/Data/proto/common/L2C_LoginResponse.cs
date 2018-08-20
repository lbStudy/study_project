using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
	public class L2C_LoginResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<AreaInfo> areas { get; set; }

        public void Dispose()
        {
            id = 0;
            errorCode = 0;
            areas = null;
        }
    }
}
