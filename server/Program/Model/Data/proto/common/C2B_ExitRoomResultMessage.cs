using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2B_ExitRoomResultMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public long roomid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public bool isLeave{set;get;}

        public void Dispose()
        {
            id = 0;
            roomid = 0;
            isLeave = false;
        }
    }
}
