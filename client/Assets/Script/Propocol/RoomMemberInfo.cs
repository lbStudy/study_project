using System;
using System.Collections.Generic;


namespace Data
{
    [ProtoBuf.ProtoContract]
    public class RoomSynchInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public long surplusTime { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int state { set; get; }
    }
    [ProtoBuf.ProtoContract]
    public class RoomMemberInfo
    {
        [ProtoBuf.ProtoMember(3)]
        public long playerid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string name{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public bool isCreator{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int configid { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public long enterTime{set;get;}
        [ProtoBuf.ProtoMember(8)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(9)]
        public bool isOnline{set;get;}
        [ProtoBuf.ProtoMember(10)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(11)]
        public bool voiceOn{set;get;}
        [ProtoBuf.ProtoMember(12)]
        public int group { set; get; }
        [ProtoBuf.ProtoMember(14)]
        public int sex{set;get;}
    }
    [ProtoBuf.ProtoContract]
    public class RoomMemberAllInfo
    {
        [ProtoBuf.ProtoMember(3)]
        public RoomMemberInfo baseInfo{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public List<byte> pais{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public List<byte> curputpais{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public int victoryCount{set;get;}
        [ProtoBuf.ProtoMember(7)]
        public int failCount{set;get;}
        [ProtoBuf.ProtoMember(8)]
        public int callVictoryCount{set;get;}
        [ProtoBuf.ProtoMember(9)]
        public int jiesuanDaoshu{set;get;}
        [ProtoBuf.ProtoMember(10)]
        public int paitype{set;get;}
        [ProtoBuf.ProtoMember(11)]
        public List<int> scores{set;get;}
        [ProtoBuf.ProtoMember(12)]
        public int point { set; get; }
        [ProtoBuf.ProtoMember(13)]
        public int canUseZhadanCount { set; get; }
        [ProtoBuf.ProtoMember(14)]
        public int curUseZhadanCount { set; get; }
        [ProtoBuf.ProtoMember(15)]
        public int paiCount { set; get; }
        [ProtoBuf.ProtoMember(16)]
        public int callFailCount { set; get; }
        [ProtoBuf.ProtoMember(17)]
        public List<byte> feiwangs { set; get; }
        [ProtoBuf.ProtoMember(18)]
        public bool isShowCount { set; get; }
    }
}
