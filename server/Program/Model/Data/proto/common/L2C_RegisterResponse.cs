using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class L2C_RegisterResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }

        public void Dispose()
        {
            errorCode = 0;
        }
    }
}
