using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class A2A_ServerPingMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public long fromApp{set;get;}

        public void Dispose()
        {
            fromApp = 0;

        }
	}
}
