using System;
using System.Collections.Generic;
using Base;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_GetRoomInfoResponse : IDisposable, IAwake
    {
		[ProtoBuf.ProtoMember(1)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public RoomSynchInfo roomSynInfo { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<RoomMemberInfo> memberInfos { get; set; }

        public void Awake()
        {
            memberInfos = new List<RoomMemberInfo>();
        }

        public void Dispose()
		{
		    errorCode = 0;
            roomSynInfo = null;
            memberInfos.Clear();
        }
	}
}
