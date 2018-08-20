using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2Gate_RemovePlayerResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }

        public void Dispose()
        {
            errorCode = 0;
        }
    }
}
