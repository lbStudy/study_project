using System;
using System.Collections.Generic;
using Base;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class A2A_ServerRegisterResponse : IDisposable, IAwake
	{
		[ProtoBuf.ProtoMember(1)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public List<InnerConnectInfo> connectInfo { get; set; }

        public void Awake()
        {
            connectInfo = new List<InnerConnectInfo>();
        }

        public void Dispose()
		{
		    errorCode = 0;
            connectInfo.Clear();
        }
	}
}
