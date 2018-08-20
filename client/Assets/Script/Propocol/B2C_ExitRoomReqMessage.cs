using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_ExitRoomReqMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public long reqPlayerid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public bool isAgree{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public bool isOpen{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public long startVoteTime { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public long voteTime { set; get; }
        public void Dispose()
        {
            reqPlayerid = 0;
            isAgree = false;
            isOpen = false;
            startVoteTime = 0;
            voteTime = 0;
        }
    }
}
