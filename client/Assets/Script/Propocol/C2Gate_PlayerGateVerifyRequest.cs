using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2Gate_PlayerGateVerifyRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public string checkcode{set;get;}

        public void Dispose()
        {
            id = 0;
            checkcode = string.Empty;
        }
    }
}
