using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public struct RoomCommonPackage
    {
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int type { set; get; }

        [ProtoBuf.ProtoMember(5)]
        public int param1 { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int param2 { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public long targetid { set; get; }
    }
}
