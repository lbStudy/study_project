using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(8697)]
    public class Gate2M_CreateRoomRequestHandler : AMRpcHandler<Gate2M_CreateRoomRequest>
    {
        protected override void Run(RpcPackage package)
        {
            Gate2M_CreateRoomRequest req = package.msg as Gate2M_CreateRoomRequest;
            M2Gate_CreateRoomResponse response = package.Response as M2Gate_CreateRoomResponse;

            try
            {
                response.battleAppid = RoomAllotComponent.Instance.GetBattleServer();
                response.id = RoomAllotComponent.Instance.CreateRoomId();

                RoomInfo info = new RoomInfo();
                info.battleAppid = response.battleAppid;
                info.curCount = 1;
                info.totalCount = req.totalCount;
                RoomAllotComponent.Instance.JoinBattleServer(info, response.id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                package.Reply();
            }
        }
    }
}
