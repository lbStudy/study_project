using Base;
using Data;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(33834)]
    public class Gate2B_ReEnterRoomRequestHandler : AMRpcHandler<Gate2B_ReEnterRoomRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            Gate2B_ReEnterRoomRequest req = pacakage.msg as Gate2B_ReEnterRoomRequest;
            B2Gate_ReEnterRoomResponse response = pacakage.Response as B2Gate_ReEnterRoomResponse;

            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if (room == null)
                {
                    response.errorCode = (int)ErrorCode.NotExist;
                    return;
                }
                RoomMember member = room.Find(req.id);
                if(member == null)
                {
                    response.errorCode = (int)ErrorCode.NotExist;
                    return;
                }
                //if(room.IsRoomEmpty())
                //{
                //    RoomManagerComponent.Instance.RemoveEmptyRoom(req.roomid);
                //}
                member.info.isOnline = true;
                response.allInfos = new List<RoomMemberAllInfo>();
                response.roomInfo = room.roomInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                pacakage.Reply();
            }
        }
    }
}
