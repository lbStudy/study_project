using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(24876)]
    public class Gate2M_PlayerOfflineMessageHandler : AMHandler<Gate2M_PlayerOfflineMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                Gate2M_PlayerOfflineMessage msg = package.msg as Gate2M_PlayerOfflineMessage;
                PlayerAllotInfo info = ServerAllotComponent.Instance.Find(msg.id);
                M2L_PlayerOfflineMessage msgToL = new M2L_PlayerOfflineMessage();
                msgToL.id = msg.id;
                //Session loginSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.LoginAppId);
                //loginSession.SendMessage(msgToL, 0);
            }
            finally
            {
                package.Dispose();
            }

        }
    }
}
