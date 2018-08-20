using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2M_StartBattleMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long gameTime { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public long roomid { get; set; }
        public void Dispose()
		{
            gameTime = 0;
            roomid = 0;
        }
	}
}
