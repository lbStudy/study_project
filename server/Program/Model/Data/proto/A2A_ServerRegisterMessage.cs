using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class A2A_ServerRegisterMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public int appId { set; get; }
        [ProtoBuf.ProtoMember(2)]
        public int appType { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int system { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public int innerPort { set; get; }
        [ProtoBuf.ProtoMember(5)]
        public string innerIp { set; get; }
        [ProtoBuf.ProtoMember(6)]
        public int outerPort { set; get; }
        [ProtoBuf.ProtoMember(7)]
        public string outerIp { set; get; }
        public void Dispose()
		{
            innerIp = string.Empty;
            outerIp = string.Empty;
        }
	}
}
