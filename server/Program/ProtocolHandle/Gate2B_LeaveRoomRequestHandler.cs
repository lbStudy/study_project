using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(50013)]
    public class Gate2B_LeaveRoomRequestHandler : AMRpcHandler<Gate2B_LeaveRoomRequest>
    {
        protected override void Run(RpcPackage package)
        {
            Gate2B_LeaveRoomRequest req = package.msg as Gate2B_LeaveRoomRequest;
            B2Gate_LeaveRoomResponse response = package.Response as B2Gate_LeaveRoomResponse;
            B2C_LeaveRoomMessage b2c = ProtocolDispatcher.Instance.Take<B2C_LeaveRoomMessage>((int)ProtoEnum.B2C_LeaveRoomMessage);
            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if(room == null)
                {
                    return;
                }
                RoomMember member = room.Find(req.playerid);
                if(member == null)
                {
                    return;
                }
                room.RemoveMember(member);
                b2c.id = req.playerid;
                room.BroadcastMsg(b2c);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                ProtocolDispatcher.Instance.Back(b2c);
                package.Reply();
            }
        }
    }
}
