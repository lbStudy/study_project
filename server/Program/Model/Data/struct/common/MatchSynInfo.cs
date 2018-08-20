using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class MatchSynInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public long playerid { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string name { get; set; }
    }
}
