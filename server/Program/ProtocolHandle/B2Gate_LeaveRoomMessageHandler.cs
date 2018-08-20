using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(66193)]
    public class B2Gate_LeaveRoomMessageHandler : AMHandler<B2Gate_LeaveRoomMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                B2Gate_LeaveRoomMessage msg = pacakage.msg as B2Gate_LeaveRoomMessage;
                Player player = PlayerManagerComponent.Instance.Find(pacakage.Toid);
                if (player != null)
                {

                }
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
