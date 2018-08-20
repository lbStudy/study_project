using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class B2Gate_GameSettingResponse : IDisposable
	{
        [ProtoBuf.ProtoMember(2)]
        public int errorCode { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public GameSettingInfo mInfo{set;get;}

        public void Dispose()
        {
            errorCode = 0;
            mInfo = null;
        }
    }
}
