using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2B_MoveRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public int t_x { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int t_y { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int t_z { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int p_x { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public int p_y { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int p_z { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public long roomid { set; get; }
        public void Dispose()
		{
            t_x = 0;
            t_y = 0;
            t_z = 0;
            p_x = 0;
            p_y = 0;
            p_z = 0;
            roomid = 0;
        }
	}
}
