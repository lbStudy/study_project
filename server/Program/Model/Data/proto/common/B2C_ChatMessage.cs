using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_ChatMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public ChatInfo mInfo{set;get;}
        public void Dispose()
        {
            mInfo = null;
        }
    }
}
