using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(41478)]
    public class C2Gate_ReloginRequestHandler : AMRpcHandler<C2Gate_ReloginRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            C2Gate_ReloginRequest req = pacakage.msg as C2Gate_ReloginRequest;
            Gate2C_ReloginResponse response = pacakage.Response as Gate2C_ReloginResponse;

            try
            {
                Player player = PlayerManagerComponent.Instance.Find(req.playerid);
                if(player == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                if(player.TemporaryData.checkCode != req.checkCode)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                player.SetState(PlayerState.Online);
                TranspondComponent.instance.Add(req.playerid, pacakage.Source);
                Gate2M_ReloginMessage msgToM = new Gate2M_ReloginMessage();
                msgToM.id = req.playerid;
                msgToM.gateAppid = Game.Instance.Appid;
                msgToM.name = player.CommonData.name;
                msgToM.iconUrl = player.TemporaryData.iconUrl;
                msgToM.sex = player.TemporaryData.sex;
                Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
                managerSession.SendMessage(msgToM, 0);
                Log.Debug($"gateÍæ¼Ò½øÈë : {req.playerid}");
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
