using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(44385)]
    public class M2A_ReloadDataConfigMessageHandler : AMHandler<M2A_ReloadDataConfigMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2A_ReloadDataConfigMessage msg = pacakage.msg as M2A_ReloadDataConfigMessage;
                FuncDispatcher.Instance.Run((int)FunctionId.LoadDataConfig, msg.configStrs);
            }
            finally
            {
                pacakage.Clear();
            }

        }
    }
}
