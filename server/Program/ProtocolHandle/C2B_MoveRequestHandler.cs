using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(15587)]
    public class C2B_MoveRequestHandler : AMRpcHandler<C2B_MoveRequest>
    {
        protected override void Run(RpcPacakage package)
        {
            C2B_MoveRequest req = package.msg as C2B_MoveRequest;
            B2C_MoveResponse response = package.Response as B2C_MoveResponse;

            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if(room == null || room.state != RoomState.Runing)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                RoomMember member = room.Find(package.Toid);
                if(member == null)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
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
