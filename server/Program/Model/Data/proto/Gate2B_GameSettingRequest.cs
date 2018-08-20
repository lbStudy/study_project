using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2B_GameSettingRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public long mRoomId{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public GameSettingInfo mInfo{set;get;}

        public void Dispose()
        {
            id = 0;
            mRoomId = 0;
            mInfo = null;
        }
    }
}
