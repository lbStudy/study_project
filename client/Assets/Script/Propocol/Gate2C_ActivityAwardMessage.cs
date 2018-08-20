using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_ActivityAwardMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int activityType { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public AwardInfo awardInfo { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public List<int> param1 { get; set; }

        public void Dispose()
        {
            id = 0;
            activityType = 0;
            awardInfo = null;
            param1 = null;
        }
    }
}
