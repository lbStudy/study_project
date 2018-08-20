using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2C_PlayerCommonDataSynchroMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public Dictionary<int, long> changeDataDic{set;get;}

        public void Dispose()
        {
            id = 0;
            changeDataDic = null;
        }
    }
}
