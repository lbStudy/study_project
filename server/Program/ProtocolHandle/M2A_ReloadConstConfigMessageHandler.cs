using Base;
using Data;


namespace ProtocolHandle
{
    [Protocol(33836)]
    public class M2A_ReloadConstConfigMessageHandler : AMHandler<M2A_ReloadConstConfigMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2A_ReloadConstConfigMessage msg = package.msg as M2A_ReloadConstConfigMessage;
                ConstConfigComponent.Instance.Load();
            }
            finally
            {
                package.Dispose();
            }

        }
    }
}
