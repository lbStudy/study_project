using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(85937)]
    public class M2L_VerifyTimeOutMessageHandler : AMHandler<M2L_VerifyTimeOutMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2L_VerifyTimeOutMessage msg = pacakage.msg as M2L_VerifyTimeOutMessage;
                for (int i = 0; i < msg.playerids.Count; i++)
                {
                    LoginInfo info = LoginManagerComponent.Instance.FindLoginInfoById(msg.playerids[i]);
                    if (info != null && !info.IsInGame)
                    {
                        if (!info.IsInGame)
                        {
                            LoginManagerComponent.Instance.RemoveLoginInfo(msg.playerids[i]);
                        }
                        info.state = LoginState.None;
                        if (info.session != null)
                        {
                            info.session.Dispose();
                        }
                    }
                }
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
