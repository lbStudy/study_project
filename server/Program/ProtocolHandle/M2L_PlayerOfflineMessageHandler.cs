using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(1112)]
    public class M2L_PlayerOfflineMessageHandler : AMHandler<M2L_PlayerOfflineMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2L_PlayerOfflineMessage msg = package.msg as M2L_PlayerOfflineMessage;
                LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(msg.id);
                if (info != null)
                {
                    info.IsInGame = false;
                    if (info.state == LoginState.None)
                    {
                        LoginManagerComponent.Instance.RemoveLoginInfo(msg.id);
                    }
                }
            }
            finally
            {
                package.Dispose();
            }
            
        }
    }
}
