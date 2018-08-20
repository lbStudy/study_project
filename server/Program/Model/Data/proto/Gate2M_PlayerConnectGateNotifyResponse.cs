using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2M_PlayerConnectGateNotifyResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public string checkcode{set;get;}

        public void Dispose()
        {
            errorCode = 0;
            checkcode = string.Empty;
        }
    }
}
