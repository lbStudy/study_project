using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2C_DealMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public long id { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public List<byte> pais{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public long callPlayerid { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public byte zhuangPai { set; get; }

        public void Dispose()
        {
            id = 0;
            pais = null;
            callPlayerid = 0;
            zhuangPai = 0;
        }
    }
}
