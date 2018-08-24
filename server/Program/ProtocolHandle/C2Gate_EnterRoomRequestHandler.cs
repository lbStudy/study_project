using Base;
using Config;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(45951)]
    public class C2Gate_EnterRoomRequestHandler : AMRpcHandler<C2Gate_EnterRoomRequest>
    {
        protected override async void Run(RpcPackage package)
        {
            C2Gate_EnterRoomRequest req = package.msg as C2Gate_EnterRoomRequest;
            Gate2C_EnterRoomResponse response = package.Response as Gate2C_EnterRoomResponse;

            Gate2B_EnterRoomRequest reqToB = ProtocolDispatcher.Instance.Take<Gate2B_EnterRoomRequest>((int)ProtoEnum.Gate2B_EnterRoomRequest);
            B2Gate_EnterRoomResponse respFromB = ProtocolDispatcher.Instance.Take<B2Gate_EnterRoomResponse>((int)ProtoEnum.B2Gate_EnterRoomResponse);
            Gate2B_LeaveRoomRequest reqToB_leave = ProtocolDispatcher.Instance.Take<Gate2B_LeaveRoomRequest>((int)ProtoEnum.Gate2B_LeaveRoomRequest);
            B2Gate_LeaveRoomResponse respFromB_leave = ProtocolDispatcher.Instance.Take<B2Gate_LeaveRoomResponse>((int)ProtoEnum.B2Gate_LeaveRoomResponse);
            try
            {
                Player player = PlayerManagerComponent.Instance.Find(package.Toid);
                if(player == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                long roomid = 0;
                Session roomSession = null;
                RoomBaseConfig roomBaseConfig = XmlDataComponent.Instance.worldMapConfig.FindRoom((int)player.Data.detailData.roomid);
                if (req.roomid == 0 || req.roomid == player.Data.detailData.roomid)
                {//进入当前房间
                    roomid = player.Data.detailData.roomid;
                }
                else
                {//切换房间
                    roomid = req.roomid;
                    //判断是否可以进入房间
                    if (roomBaseConfig.nearDic.ContainsKey(roomid) == false)
                    {
                        response.errorCode = (int)ErrorCode.Fail;
                        return;
                    }
                    RoomBaseConfig rbconfig = XmlDataComponent.Instance.worldMapConfig.FindRoom(roomid);
                    if (rbconfig == null)
                    {
                        response.errorCode = (int)ErrorCode.Fail;
                        return;
                    }
                    //离开当前房间
                    reqToB_leave.playerid = player.Id;
                    reqToB_leave.roomid = player.Data.detailData.roomid;
                    //roomSession = NetInnerComponent.Instance.GetByAppID(roomBaseConfig.serverid);
                    //respFromB_leave = await roomSession.Call(reqToB_leave, respFromB_leave) as B2Gate_LeaveRoomResponse;
                    //if(respFromB_leave.errorCode != (int)ErrorCode.Success)
                    //{
                    //    response.errorCode = respFromB_leave.errorCode;
                    //    return;
                    //}
                }
                //进入选择的房间
                reqToB.id = player.Id;
                reqToB.roomid = roomid;
                reqToB.fromAppid = Game.Instance.Appid;
                reqToB.name = player.Data.detailData.name;
                reqToB.iconUrl = player.TemporaryData.iconUrl;
                reqToB.sex = player.TemporaryData.sex;
                
                //roomSession = NetInnerComponent.Instance.GetByAppID(roomBaseConfig.serverid);
                //if (roomSession == null)
                //{
                //    Log.Debug($"Not exist roomsession, serverid : {roomBaseConfig.serverid}");
                //    response.errorCode = (int)ErrorCode.Fail;
                //    return;
                //}
                //respFromB = await roomSession.Call(reqToB, respFromB) as B2Gate_EnterRoomResponse;
                //if (respFromB.errorCode != (int)ErrorCode.Success)
                //{
                //    response.errorCode = respFromB.errorCode;
                //    return;
                //}
                player.TemporaryData.SetBattleAppid(roomBaseConfig.serverid);
                player.CommonData.SetRoomid(roomid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {
                ProtocolDispatcher.Instance.Back(reqToB_leave);
                ProtocolDispatcher.Instance.Back(respFromB_leave);
                ProtocolDispatcher.Instance.Back(reqToB);
                ProtocolDispatcher.Instance.Back(respFromB);
                package.Reply();
            }
        }
    }
}
