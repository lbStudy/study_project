using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class M2A_ReloadDataConfigMessage : IDisposable
    {
        [ProtoBuf.ProtoMember(3)]
        public List<int> configStrs{set;get;}

        public void Dispose()
        {
            configStrs = null;
        }
    }
}
