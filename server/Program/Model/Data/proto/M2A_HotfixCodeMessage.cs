using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2A_HotfixCodeMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(3)]
        public int module{set;get;}

        public void Dispose()
        {
            module = 0;
        }
    }
}
