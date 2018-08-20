using System;

namespace Data
{
	[ProtoBuf.ProtoContract]
	public class B2C_StateChangeMessage : IDisposable
	{
        [ProtoBuf.ProtoMember(1)]
        public long characterid { set; get; }
        [ProtoBuf.ProtoMember(3)]
        public int state { set; get; }
        [ProtoBuf.ProtoMember(4)]
        public float stateTime { set; get; }
        [ProtoBuf.ProtoMember(14)]
        public long param0 { set; get; }
        [ProtoBuf.ProtoMember(15)]
        public int param1 { set; get; }
        [ProtoBuf.ProtoMember(16)]
        public int param2 { set; get; }
        [ProtoBuf.ProtoMember(17)]
        public int param3 { set; get; }
        public void Awake()
        {

        }

        public void Dispose()
        {
            characterid = 0;
            state = 0;
            param1 = 0;
            param2 = 0;
            param0 = 0;
            stateTime = 0;
            param3 = 0;
        }
    }
}
