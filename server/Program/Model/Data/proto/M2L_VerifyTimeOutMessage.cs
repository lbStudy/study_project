using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2L_VerifyTimeOutMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public List<long> playerids{set;get;}

        public void Dispose()
        {
            playerids = null;
        }
    }
}
