using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2L_ReloginRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public string account{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string password{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public int areaid{set;get;}

        public void Dispose()
        {
            account = string.Empty;
            password = string.Empty;
            areaid = 0;
        }
    }
}
