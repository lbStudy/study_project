using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_GetActivityInfoResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public List<ActivityInfo> activitys;

        public void Dispose()
        {
            errorCode = 0;
            activitys = null;
        }
    }
}
