
namespace Data
{
    [ProtoBuf.ProtoContract]
    public class GameSettingInfo
    {
        public enum SETTING_TYPE
        {
            VOICE_STATUS,
        }
        [ProtoBuf.ProtoMember(3)]
        public SETTING_TYPE mType{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public byte[] mData{set;get;}
    }
}
