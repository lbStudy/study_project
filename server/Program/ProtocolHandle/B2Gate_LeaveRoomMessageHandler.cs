using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(66193)]
    public class B2Gate_LeaveRoomMessageHandler : AMHandler<B2Gate_LeaveRoomMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                B2Gate_LeaveRoomMessage msg = package.msg as B2Gate_LeaveRoomMessage;
                Player player = PlayerManagerComponent.Instance.Find(package.Toid);
                if (player != null)
                {

                }
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
