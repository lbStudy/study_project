using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(97911)]
    public class C2B_ExitRoomResultMessageHandler : AMHandler<C2B_ExitRoomResultMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                C2B_ExitRoomResultMessage msg = package.msg as C2B_ExitRoomResultMessage;
                Room room = RoomManagerComponent.Instance.Find(msg.roomid);
                if (room == null)
                {
                    return;
                }
                RoomMember member = room.Find(msg.id);
                if (member == null)
                {
                    return;
                }
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
