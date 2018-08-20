using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2L_RegisterRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public string account{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string password{set;get;}

        public void Dispose()
        {
            account = string.Empty;
            password = string.Empty;
        }
    }
}
