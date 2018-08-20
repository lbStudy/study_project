namespace Data
{
    [ProtoBuf.ProtoContract]
    public class AreaInfo
    {
        [ProtoBuf.ProtoMember(3)]
        public int id{ get; set; }
        [ProtoBuf.ProtoMember(4)]
        public string name { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int enterCount { get; set; }
        [ProtoBuf.ProtoMember(6)]
        public bool isOpen { get; set; }
    }
}