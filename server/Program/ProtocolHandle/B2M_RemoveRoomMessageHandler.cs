using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(14568)]
    public class B2M_RemoveRoomMessageHandler : AMHandler<B2M_RemoveRoomMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            B2M_RemoveRoomMessage msg = pacakage.msg as B2M_RemoveRoomMessage;
            try
            {
                RoomAllotComponent.Instance.RemoveRoom(msg.battleAppid, msg.id);
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
