using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_UseAtkMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int skillid { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int t_x { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int t_y { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public int t_z { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int param1 { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public int param2 { set; get; }
        [ProtoBuf.ProtoMember(8)]
        public int param3 { set; get; }
        [ProtoBuf.ProtoMember(9)]
        public int param4 { set; get; }
        [ProtoBuf.ProtoMember(10)]
        public long targetid { set; get; }
        public void Dispose()
        {
            characterid = 0;
            skillid = 0;
            t_x = 0;
            t_y = 0;
            t_z = 0;
            param1 = 0;
            param2 = 0;
            param3 = 0;
            param4 = 0;
            targetid = 0;
        }
	}
}
