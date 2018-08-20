using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2Gate_ReloginRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public long playerid{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public string checkCode{set;get;}

        public void Dispose()
        {
            playerid = 0;
            checkCode = string.Empty;
        }
    }
}
