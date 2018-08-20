using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class SkillInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public int skillid;
        [ProtoBuf.ProtoMember(2)]
        public float cd;
        [ProtoBuf.ProtoMember(3)]
        public float passTime;
        [ProtoBuf.ProtoMember(4)]
        public bool isDoing;
        [ProtoBuf.ProtoMember(5)]
        public float param1;
        [ProtoBuf.ProtoMember(6)]
        public int param2;
    }
}
