using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_GlobalNoticeMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(3)]
        public string content{set;get;}

        public void Dispose()
        {
            content = string.Empty;
        }
    }
}
