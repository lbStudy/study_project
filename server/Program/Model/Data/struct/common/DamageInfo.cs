using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class DamageInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public long targetid { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int score { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int damage { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public int param1 { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int param2 { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public int param3 { get; set; }
    }
}
