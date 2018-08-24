using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(14568)]
    public class B2M_RemoveRoomMessageHandler : AMHandler<B2M_RemoveRoomMessage>
    {
        protected override void Run(MsgPackage package)
        {
            B2M_RemoveRoomMessage msg = package.msg as B2M_RemoveRoomMessage;
            try
            {
                RoomAllotComponent.Instance.RemoveRoom(msg.battleAppid, msg.id);
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
