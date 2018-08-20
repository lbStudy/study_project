using Base;
using System;
using System.Collections.Generic;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_DamageMessage : IDisposable, IAwake
	{
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int skillid { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int effect { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int param1 { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public long sourceid { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int damageVal { set; get; }

        public void Awake()
        {
            
        }

        public void Dispose()
		{
            characterid = 0;
            skillid = 0;
            effect = 0;
            param1 = 0;
            sourceid = 0;
            damageVal = 0;
        }
	}
}
