using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(38322)]
    public class Gate2M_VerifyTimeOutMessageHandler : AMHandler<Gate2M_VerifyTimeOutMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                Gate2M_VerifyTimeOutMessage msg = package.msg as Gate2M_VerifyTimeOutMessage;
                M2L_VerifyTimeOutMessage msgToL = new M2L_VerifyTimeOutMessage();
                //msgToL.playerids = msg.playerids;
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
