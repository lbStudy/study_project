using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_GmFindPlayerInfoRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public List<long> playerids { get; set; }

        public void Dispose()
        {
            playerids = null;

        }
    }
}
