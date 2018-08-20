using System.Collections.Generic;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class PlayerDetailData
    {
        [ProtoBuf.ProtoMember(3)]
        public long playerid { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public string name { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int roomcard { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public bool voiceOpen { get; set; } = true;
        [ProtoBuf.ProtoMember(7)]
        public long firstLoginTime { get; set; }
        [ProtoBuf.ProtoMember(8)]
        public List<int> finish7DayAwards { get; set; }
        [ProtoBuf.ProtoMember(9)]
        public int level { get; set; }
        [ProtoBuf.ProtoMember(10)]
        public long roomid{ get; set; }
    }
}
