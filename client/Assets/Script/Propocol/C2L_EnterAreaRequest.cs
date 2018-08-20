using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2L_EnterAreaRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public int areaid{set;get;}

        public void Dispose()
        {
            areaid = 0;
        }
    }
}
