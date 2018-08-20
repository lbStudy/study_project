using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2B_UseAtkRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long roomid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int skillid { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int p_x { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int p_y { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public int p_z { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int param1 { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public int param2 { set; get; }
        [ProtoBuf.ProtoMember(8)]
        public int param3 { set; get; }
        [ProtoBuf.ProtoMember(9)]
        public int param4 { set; get; }
        [ProtoBuf.ProtoMember(10)]
        public int param5 { set; get; }
        public void Dispose()
		{
            roomid = 0;
            skillid = 0;
            p_x = 0;
            p_y = 0;
            p_z = 0;
            param1 = 0;
            param2 = 0;
            param3 = 0;
            param4 = 0;
            param5 = 0;
        }
	}
}
