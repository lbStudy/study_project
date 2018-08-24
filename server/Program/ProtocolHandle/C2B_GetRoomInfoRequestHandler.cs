using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(81594)]
    public class C2B_GetRoomInfoRequestHandler : AMRpcHandler<C2B_GetRoomInfoRequest>
    {
        protected override void Run(RpcPackage package)
        {
            C2B_GetRoomInfoRequest req = package.msg as C2B_GetRoomInfoRequest;
            B2C_GetRoomInfoResponse response = package.Response as B2C_GetRoomInfoResponse;
            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if (room == null)
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
                response.roomSynInfo = room.roomInfo;
                room.GetMemberInfos(response.memberInfos, 0);
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
