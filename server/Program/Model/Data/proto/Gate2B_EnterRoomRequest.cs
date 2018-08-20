using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2B_EnterRoomRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public long roomid{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public int fromAppid{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public string name{set;get;}
        [ProtoBuf.ProtoMember(10)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(11)]
        public int sex{set;get;}

        public void Dispose()
        {
            id = 0;
            roomid = 0;
            fromAppid = 0;
            name = string.Empty;
            iconUrl = string.Empty;
            sex = 0;
        }
    }
}
