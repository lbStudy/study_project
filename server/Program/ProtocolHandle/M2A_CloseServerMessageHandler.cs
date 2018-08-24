using Base;
using Data;

namespace ProtocolHandle
{
    [Protocol(53355)]
    public class M2A_CloseServerMessageHandler : AMHandler<M2A_CloseServerMessage>
    {
        protected override void Run(MsgPackage package)
        {
            try
            {
                M2A_CloseServerMessage msg = package.msg as M2A_CloseServerMessage;
                if (Game.Instance.AppType == AppType.GateServer)
                {
                    PlayerManagerComponent.Instance.OpenQuickSave();
                }
                else if (Game.Instance.AppType == AppType.BattleServer)
                {
                    RoomManagerComponent.Instance.CloseAllRoom();
                }
            }
            finally
            {
                package.Dispose();
            }
        }
    }
}
