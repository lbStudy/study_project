using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_BattleResultMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public int maxScore { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int rank { get; set; }
        public void Dispose()
        {
            maxScore = 0;
            rank = 0;
        }
    }
}