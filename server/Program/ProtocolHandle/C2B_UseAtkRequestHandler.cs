using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(38833)]
    public class C2B_UseAtkRequestHandler : AMRpcHandler<C2B_UseAtkRequest>
    {
        protected override void Run(RpcPackage package)
        {
            C2B_UseAtkRequest req = package.msg as C2B_UseAtkRequest;
            B2C_UseAtkResponse response = package.Response as B2C_UseAtkResponse;
            
            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if (room == null || room.state != RoomState.Runing)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                RoomMember member = room.Find(package.Toid);
                if (member == null)
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
