using System.Collections.Generic;
using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class Gate2C_GetPlayerInfoResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public PlayerDetailData detail{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public PlayerExtraData extra{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public long roomid{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public string iconUrl{set;get;}
        [ProtoBuf.ProtoMember(7)]
        public int sex{set;get;}
        [ProtoBuf.ProtoMember(8)]
        public int sceneid { set; get; }
        [ProtoBuf.ProtoMember(9)]
        public string roomIp { set; get; }
        [ProtoBuf.ProtoMember(10)]
        public int roomPort { set; get; }
        [ProtoBuf.ProtoMember(11)]
        public string roomCheckcode { set; get; }

        public void Dispose()
        {
            errorCode = 0;
            detail = null;
            extra = null;
            roomid = 0;
            iconUrl = string.Empty;
            sex = 0;
        }
    }
}
