using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2C_PlayerGateVerifyResponse// : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }

        public void Dispose()
        {
            errorCode = 0;
        }
    }
}
