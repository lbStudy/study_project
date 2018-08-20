
using System.Collections.Generic;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class BattleResultInfo
    {
        [ProtoBuf.ProtoMember(3)]
        public long playerid{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public List<byte> pais{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int oneJiesuanDaoshu{set;get;}
        [ProtoBuf.ProtoMember(7)]
        public int victoryCount{set;get;}
        [ProtoBuf.ProtoMember(8)]
        public int failCount{set;get;}
        [ProtoBuf.ProtoMember(9)]
        public int callVictoryCount { set;get;}
        [ProtoBuf.ProtoMember(10)]
        public int jiesuanDaoshu{set;get;}
        [ProtoBuf.ProtoMember(11)]
        public int callFailCount { set; get; }
    }
}
