using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(87657)]
    public class C2B_ChatMessageHandler : AMHandler<C2B_ChatMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                C2B_ChatMessage msg = package.msg as C2B_ChatMessage;
                if (null != msg.mInfo)
                {
                    Room room = RoomManagerComponent.Instance.Find(msg.mInfo.mRoomId);
                    if (null != room)
                    {
                        RoomMember me = room.Find(msg.id);
                        if (null != me)
                        {
                            B2C_ChatMessage msg2c = new B2C_ChatMessage();
                            msg2c.mInfo = msg.mInfo;
                            room.BroadCastChat(msg2c);
                        }
                    }
                }
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
