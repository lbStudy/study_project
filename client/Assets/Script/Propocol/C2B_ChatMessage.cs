using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2B_ChatMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public ChatInfo mInfo{set;get;}

        public void Dispose()
        {
            id = 0;
            mInfo = null;
        }
    }
}
