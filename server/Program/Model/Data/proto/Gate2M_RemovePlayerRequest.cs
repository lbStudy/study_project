using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_RemovePlayerRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(3)]
        public List<long> playerids{set;get;}

        public void Dispose()
        {
            playerids = null;
        }
    }
}
