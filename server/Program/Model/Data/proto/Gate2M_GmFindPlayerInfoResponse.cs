using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_GmFindPlayerInfoResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<PlayerDetailData> playerdatas { get; set; }

        public void Dispose()
        {
            errorCode = 0;
            playerdatas = null;
        }
    }
}
