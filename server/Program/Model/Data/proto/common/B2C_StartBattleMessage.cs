using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_StartBattleMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public RoomSynchInfo roomInfo { get; set; }

        public void Dispose()
		{
            roomInfo = null;
        }
	}
}
