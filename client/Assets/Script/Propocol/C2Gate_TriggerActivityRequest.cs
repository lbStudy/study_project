using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2Gate_TriggerActivityRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int activityid { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public int param1 { get; set; }

        public void Dispose()
        {
            activityid = 0;
            param1 = 0;
        }
    }
}
