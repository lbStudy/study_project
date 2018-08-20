using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_RefreshPlayerActivityMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public List<PlayerActivityInfo> playerActivitys { set; get; }

        public void Dispose()
        {
            id = 0;
            playerActivitys = null;
        }
    }
}
