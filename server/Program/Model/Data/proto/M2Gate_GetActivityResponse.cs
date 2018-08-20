using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class M2Gate_GetActivityResponse : IDisposable
    {
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<ActivityInfo> activitys { get; set; }

        public void Dispose()
        {
            errorCode = 0;
            activitys = null;
        }
    }
}
