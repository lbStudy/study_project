using Base;
using Data;
using System;
using Config;

namespace ProtocolHandle
{
    [Protocol(38039)]
    public class C2Gate_PlayerGateVerifyRequestHandler : AMRpcHandler<C2Gate_PlayerGateVerifyRequest>
    {
        protected override async void Run(RpcPacakage pacakage)
        {
            C2Gate_PlayerGateVerifyRequest req = pacakage.msg as C2Gate_PlayerGateVerifyRequest;
            Gate2C_PlayerGateVerifyResponse response = pacakage.Response as Gate2C_PlayerGateVerifyResponse;
            Gate2M_PlayerEnterGameRequest reqToM = ProtocolDispatcher.Instance.Take<Gate2M_PlayerEnterGameRequest>((int)ProtoEnum.Gate2M_PlayerEnterGameRequest);
            M2Gate_PlayerEnterGameResponse respFromM = ProtocolDispatcher.Instance.Take<M2Gate_PlayerEnterGameResponse>((int)ProtoEnum.M2Gate_PlayerEnterGameResponse);


            try
            {
                VerifyInfo info = VerifyComponent.Instance.Find(req.id);
                if(info == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                if(info.checkCode != req.checkcode)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                Player player = PlayerManagerComponent.Instance.Find(req.id);
                PlayerData playerData = null;
                bool isInit = false;
                if (player == null)
                {
                    if (DBOperateComponent.Instance.IsConnect())
                        playerData = await DBOperateComponent.Instance.FindPlayerDataAsync(req.id);
                    else
                    {
                        response.errorCode = (int)ErrorCode.DB_Disconnect;
                        return;
                    }
                    player = new Player(req.id);
                    isInit = true;
                }
                else
                {
                    playerData = player.Data;
                }

                if (playerData == null)
                {
                    playerData = new PlayerData();
                    playerData.id = req.id;
                    playerData.detailData = new PlayerDetailData();
                    playerData.detailData.playerid = req.id;
                    playerData.detailData.roomcard = ConstConfigComponent.ConstConfig.InitRoomCard;
                    playerData.detailData.roomid = 1000;
                    playerData.detailData.firstLoginTime = Game.Instance.Sec;
                    playerData.extraData = new PlayerExtraData();
                    if (DBOperateComponent.Instance.IsConnect())
                    {
                        await DBOperateComponent.Instance.InsertPlayerDataAsync(playerData);
                    }
                    else
                    {
                        response.errorCode = (int)ErrorCode.DB_Disconnect;
                        return;
                    }
                }

                reqToM.id = req.id;
                reqToM.gateAppid = Game.Instance.Appid;
                Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
                respFromM = await managerSession.Call(reqToM, respFromM) as M2Gate_PlayerEnterGameResponse;
                if (respFromM.errorCode != (int)ErrorCode.Success)
                {
                    response.errorCode = respFromM.errorCode;
                    return;
                }

                if (isInit)
                {
                    player.Init(playerData);
                }
                player.TemporaryData.checkCode = req.checkcode;
                player.TemporaryData.iconUrl = respFromM.iconUrl;
                player.TemporaryData.sex = respFromM.sex;
                
                playerData.detailData.name = respFromM.name;
                player.SetState(PlayerState.Online);
                VerifyComponent.Instance.Remove(req.id);
                TranspondComponent.instance.Add(req.id, pacakage.Source);
                PlayerManagerComponent.Instance.Add(player);

                Log.Debug($"gateÍæ¼Ò½øÈë : {req.id}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                response.errorCode = (int)ErrorCode.CodeError;
            }
            finally
            {

                ProtocolDispatcher.Instance.Back(reqToM);
                ProtocolDispatcher.Instance.Back(respFromM);
                pacakage.Reply();
            }
        }
    }
}
