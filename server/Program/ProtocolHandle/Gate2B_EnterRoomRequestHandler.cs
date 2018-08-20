using Base;
using Data;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(28023)]
    public class Gate2B_EnterRoomRequestHandler : AMRpcHandler<Gate2B_EnterRoomRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            Gate2B_EnterRoomRequest req = pacakage.msg as Gate2B_EnterRoomRequest;
            B2Gate_EnterRoomResponse response = pacakage.Response as B2Gate_EnterRoomResponse;
            B2C_RoomMemberSyncMessage msgToc = ProtocolDispatcher.Instance.Take<B2C_RoomMemberSyncMessage>((int)ProtoEnum.B2C_RoomMemberSyncMessage);
            try
            {
                Room room = RoomManagerComponent.Instance.Find(req.roomid);
                if (room == null)
                {
                    response.errorCode = (int)ErrorCode.NotExist;
                    return;
                }
                RoomMember member = room.Find(req.id);
                if (member == null)
                {
                    member = new RoomMember();
                    member.info = new RoomMemberInfo();
                }
                member.fromGateId = req.fromAppid;
                member.info.playerid = req.id;
                member.info.name = req.name;
                member.info.enterTime = Game.Instance.Msec;
                member.info.isOnline = true;
                member.info.iconUrl = req.iconUrl;
                member.info.sex = req.sex;

                msgToc.members.Add(member.info);
                room.BroadcastMsg(msgToc, member.info.playerid);

                room.AddMember(member);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                ProtocolDispatcher.Instance.Back(msgToc);
                pacakage.Reply();
            }
        }
    }
}
