using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2C_GlobalNoticeMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public string content{set;get;}

        public void Dispose()
        {
            id = 0;
            content = string.Empty;
        }
    }
}
