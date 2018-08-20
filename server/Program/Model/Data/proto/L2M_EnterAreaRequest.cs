using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class L2M_EnterAreaRequest : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        public void Dispose()
        {
            id = 0;
        }
    }
}
