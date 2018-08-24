using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(96627)]
    public class M2A_HotfixCodeMessageHandler : AMHandler<M2A_HotfixCodeMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2A_HotfixCodeMessage msg = package.msg as M2A_HotfixCodeMessage;
                FuncDispatcher.Instance.Run((int)FunctionId.LoadHotfixModule, msg.module);
            }
            finally
            {
                package.Dispose();
            }

        }
    }
}
