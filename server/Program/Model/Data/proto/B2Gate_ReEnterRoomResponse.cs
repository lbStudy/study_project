using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2Gate_ReEnterRoomResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<RoomMemberAllInfo> allInfos{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public RoomSynchInfo roomInfo { set; get; }

        public void Dispose()
        {
            errorCode = 0;
            allInfos = null;
            roomInfo = null;
        }
    }
}
