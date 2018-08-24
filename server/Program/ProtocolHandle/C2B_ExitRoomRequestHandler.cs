using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(51337)]
    public class C2B_ExitRoomRequestHandler : AMRpcHandler<C2B_ExitRoomRequest>
    {
        protected override void Run(RpcPackage package)
        {
            C2B_ExitRoomRequest req = package.msg as C2B_ExitRoomRequest;
            B2C_ExitRoomResponse response = package.Response as B2C_ExitRoomResponse;
            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if(room == null)
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
