using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_RefreshActivityMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public List<ActivityInfo> activitys { set; get; }

        public void Dispose()
        {
            activitys = null;
        }
    }
}
