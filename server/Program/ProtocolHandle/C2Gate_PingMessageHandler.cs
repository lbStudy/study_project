using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(10083)]
    public class C2Gate_PingMessageHandler : AMHandler<C2Gate_PingMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                C2Gate_PingMessage msg = pacakage.msg as C2Gate_PingMessage;
                TranspondInfo info = TranspondComponent.instance.Find(pacakage.Toid);
                if (info == null)
                {
                    return;
                }
                info.clientSession.pingTime = Game.Instance.Msec;
            }
            finally
            {
                pacakage.Clear();
            }
        }
    }
}
