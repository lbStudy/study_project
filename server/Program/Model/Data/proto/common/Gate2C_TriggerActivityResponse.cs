using System.Collections.Generic;
using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class Gate2C_TriggerActivityResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public AwardInfo awardInfo { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public List<int> param1 { get; set; }
        public void Dispose()
        {
            errorCode = 0;
            awardInfo = null;
            param1 = null;
        }
    }
}
