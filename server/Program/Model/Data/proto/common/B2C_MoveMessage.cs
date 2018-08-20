using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_MoveMessage : IDisposable
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
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(8)]
        public int speed { set; get; }
        public void Dispose()
		{
            t_x = 0;
            t_y = 0;
            t_z = 0;
            p_x = 0;
            p_y = 0;
            p_z = 0;
            characterid = 0;
            speed = 0;
        }
	}
}
