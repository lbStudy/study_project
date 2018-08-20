using Base;
using System;
using System.Collections.Generic;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_RoomCommonPackageMessage : IDisposable, IAwake
	{
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public Dictionary<int, int> attributeDic { set; get; }

        public void Awake()
        {
            attributeDic = new Dictionary<int, int>();
        }

        public void Dispose()
		{
            attributeDic.Clear();
            characterid = 0;
        }
	}
}
