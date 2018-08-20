using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(73962)]
    public class Gate2B_PlayerOfflineMessageHandler : AMHandler<Gate2B_PlayerOfflineMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                Gate2B_PlayerOfflineMessage msg = pacakage.msg as Gate2B_PlayerOfflineMessage;
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
                //member.info.isOnline = false;
                //if (room.IsRoomEmpty())
                //{
                //    RoomManagerComponent.Instance.AddEmptyRoom(msg.roomid);
                //}
                //B2C_RoomMemberSyncMessage msgToC = new B2C_RoomMemberSyncMessage();
                //msgToC.info = member.info;
                //room.BroadcastMsg(msgToC);
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
