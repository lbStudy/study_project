using System;
using System.Collections.Generic;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class PlayerExtraData
    {
        [ProtoBuf.ProtoMember(4)]
        public List<PlayerActivityInfo> activitys { set; get; }
    }
}
