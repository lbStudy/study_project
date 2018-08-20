using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class C2Gate_RequestOrderRequest : IDisposable
	{
		[ProtoBuf.ProtoMember(3)]
        public int SType { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public int Gem { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public string ext { get; set; }

        public void Dispose()
        {
            SType = 0;
            Gem = 0;
            ext = string.Empty;
        }
    }
}
