using Base;
using Data;


namespace ProtocolHandle
{
    [Protocol(33836)]
    public class M2A_ReloadConstConfigMessageHandler : AMHandler<M2A_ReloadConstConfigMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2A_ReloadConstConfigMessage msg = pacakage.msg as M2A_ReloadConstConfigMessage;
                ConstConfigComponent.Instance.Load();
            }
            finally
            {
                pacakage.Clear();
            }

        }
    }
}
