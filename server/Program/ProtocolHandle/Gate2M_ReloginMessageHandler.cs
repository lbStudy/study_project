using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(47292)]
    public class Gate2M_ReloginMessageHandler : AMHandler<Gate2M_ReloginMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                Gate2M_ReloginMessage msg = package.msg as Gate2M_ReloginMessage;
                ServerAllotComponent.Instance.PlayerEnter(msg.id, msg.gateAppid);
                M2L_ReloginMessage msgToL = new M2L_ReloginMessage();
                msgToL.id = msg.id;
                msgToL.areaid = Game.Instance.BigAreaId;
                msgToL.name = msg.name;
                msgToL.iconUrl = msg.iconUrl;
                msgToL.sex = msg.sex;
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
