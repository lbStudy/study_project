using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2L_LoginRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public string account{ get; set; }
        [ProtoBuf.ProtoMember(4)]
        public string password { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public int channel { get; set; }

        public void Dispose()
        {
            account = string.Empty;
            password = string.Empty;
            channel = 0;
        }
    }
}
