using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(96627)]
    public class M2A_HotfixCodeMessageHandler : AMHandler<M2A_HotfixCodeMessage>
    {
        protected override void Run(MsgPacakage pacakage)
        {
            try
            {
                M2A_HotfixCodeMessage msg = pacakage.msg as M2A_HotfixCodeMessage;
                FuncDispatcher.Instance.Run((int)FunctionId.LoadHotfixModule, msg.module);
            }
            finally
            {
                pacakage.Clear();
            }

        }
    }
}
