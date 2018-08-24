using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(44385)]
    public class M2A_ReloadDataConfigMessageHandler : AMHandler<M2A_ReloadDataConfigMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2A_ReloadDataConfigMessage msg = package.msg as M2A_ReloadDataConfigMessage;
                FuncDispatcher.Instance.Run((int)FunctionId.LoadDataConfig, msg.configStrs);
            }
            finally
            {
                package.Dispose();
            }

        }
    }
}
