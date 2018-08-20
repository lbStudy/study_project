
namespace Data
{
    [ProtoBuf.ProtoContract]
    public class ChatInfo
    {
        public enum CHAT_TYPE
        {
            STRING,
            EXPRESSION,
            AUDIO,
            SETTING_WORLD,
        }
        [ProtoBuf.ProtoMember(3)]
        public long mId{set;get;}
        [ProtoBuf.ProtoMember(4)]
        public long mRoomId{set;get;}
        [ProtoBuf.ProtoMember(5)]
        public CHAT_TYPE mType{set;get;}
        [ProtoBuf.ProtoMember(6)]
        public byte[] mData{set;get;}

    }
}
