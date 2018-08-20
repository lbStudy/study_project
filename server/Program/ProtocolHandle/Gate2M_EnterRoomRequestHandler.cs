using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(93159)]
    public class Gate2M_EnterRoomRequestHandler : AMRpcHandler<Gate2M_EnterRoomRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            Gate2M_EnterRoomRequest req = pacakage.msg as Gate2M_EnterRoomRequest;
            M2Gate_EnterRoomResponse response = pacakage.Response as M2Gate_EnterRoomResponse;

            try
            {
                RoomInfo info = null;
                if (req.id == 0)
                {
                    info = RoomAllotComponent.Instance.GetRandomRoom();
                    if(info == null)
                    {
                        info = new RoomInfo();
                        info.roomid = RoomAllotComponent.Instance.CreateRoomId();
                        info.sceneid = 1;
                        info.battleAppid = RoomAllotComponent.Instance.GetBattleServer();
                        info.curCount = 0;
                        info.totalCount = ConstConfigComponent.ConstConfig.TotalMemberCountInRoom;
                        RoomAllotComponent.Instance.JoinBattleServer(info, info.roomid);
                    }
                }
                else
                {
                    info = RoomAllotComponent.Instance.FindRoom(req.id);
                }
                
                if(info == null)
                {
                    response.errorCode = (int)ErrorCode.NotExist;
                    return;
                }
                if(info.curCount >= info.totalCount)
                {
                    response.errorCode = (int)ErrorCode.Fill;
                    return;
                }
                info.curCount++;
                response.battleAppid = info.battleAppid;
                response.roomid = info.roomid;
                response.sceneid = info.sceneid;
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
