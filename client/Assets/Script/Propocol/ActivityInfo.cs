using System;
using System.Collections.Generic;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class PlayerActivityInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public int id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public List<int> changeParam1 { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<int> changeParam2 { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public List<int> changeParam3 { get; set; }
    }
    [ProtoBuf.ProtoContract]
    public class ActivityInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public int id { get; set; }//获得id
        [ProtoBuf.ProtoMember(2)]
        public int type { get; set; }//获得类型
        [ProtoBuf.ProtoMember(3)]
        public string startTime { get; set; }//开启时间
        [ProtoBuf.ProtoMember(4)]
        public string endTime { get; set; }//结束时间
        [ProtoBuf.ProtoMember(5)]
        public long StartTime { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public long EndTime { get; set; }
        [ProtoBuf.ProtoMember(7)]
        public string des { get; set; }//描述
        [ProtoBuf.ProtoMember(8)]
        public string title { get; set; }//标题
        [ProtoBuf.ProtoMember(9)]
        public int op { get; set; }//操作
        [ProtoBuf.ProtoMember(10)]
        public List<int> params1 { get; set; }//参数

        [ProtoBuf.ProtoMember(11)]
        public ResInfo ress { get; set; }//资源

    }
    [ProtoBuf.ProtoContract]
    public class ResInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public List<AwardInfo> awards { get; set; }//奖励
        [ProtoBuf.ProtoMember(2)]
        public List<int> params1 { get; set; }//参数
    }
    [ProtoBuf.ProtoContract]
    public class AwardInfo
    {
        [ProtoBuf.ProtoMember(1)]
        public List<int> itemids { get; set; }//奖励id
        [ProtoBuf.ProtoMember(2)]
        public List<int> counts { get; set; }//奖励数量
        [ProtoBuf.ProtoMember(3)]
        public List<int> params1 { get; set; }//参数

    }
}
