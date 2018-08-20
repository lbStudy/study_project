using Base;
using System;
using System.Collections.Generic;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_PickupItemMessage : IDisposable, IAwake
	{
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public List<long> items { set; get; }

        public void Awake()
        {
            items = new List<long>();
        }

        public void Dispose()
		{
            items.Clear();
            characterid = 0;
        }
	}
}
