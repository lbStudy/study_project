using System;

namespace Data
{
    [ProtoBuf.ProtoContract]
    public class C2Gate_GameSettingRequest : IDisposable
	{
        [ProtoBuf.ProtoMember(3)]
        public GameSettingInfo mInfo{set;get;}

        public void Dispose()
        {
            mInfo = null;
        }
    }
}
