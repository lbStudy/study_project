using System;
using System.Collections.Generic;
using Base;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class A2A_ServerConnectMessage : IDisposable, IAwake
    {
        [ProtoBuf.ProtoMember(1)]
        public List<InnerConnectInfo> connectInfo { get; set; }

        public void Awake()
        {
            connectInfo = new List<InnerConnectInfo>();
        }

        public void Dispose()
        {
            connectInfo.Clear();
        }
	}
}
