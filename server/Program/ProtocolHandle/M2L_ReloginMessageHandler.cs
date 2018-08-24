using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(17474)]
    public class M2L_ReloginMessageHandler : AMHandler<M2L_ReloginMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2L_ReloginMessage msg = package.msg as M2L_ReloginMessage;
                LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(msg.id);
                if (info != null)
                {
                    info.IsInGame = true;
                }
                else
                {
                    info = LoginManagerComponent.Instance.FinishLogin(msg.id);
                    info.areaid = msg.areaid;
                    info.id = msg.id;
                    info.Name = msg.name;
                    info.iconUrl = msg.iconUrl;
                    info.sex = msg.sex;
                    info.IsInGame = true;
                }
            }
            finally
            {
                package.Dispose();
            }
            
        }
    }
}
