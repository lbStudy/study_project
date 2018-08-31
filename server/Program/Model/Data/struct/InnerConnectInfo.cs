using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class InnerConnectInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public int appId { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int appType { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int system { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int innerPort { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public string innerIp { set; get; }
    }
}
