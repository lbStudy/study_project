using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(95049)]
    public class C2Gate_GameSettingRequestHandler : AMRpcHandler<C2Gate_GameSettingRequest>
    {
        protected override async void Run(RpcPacakage pacakage)
        {
            C2Gate_GameSettingRequest req = pacakage.msg as C2Gate_GameSettingRequest;
            Gate2C_GameSettingResponse response = pacakage.Response as Gate2C_GameSettingResponse;

            try
            {

                var player =  PlayerManagerComponent.Instance.Find(pacakage.Toid);
                switch(req.mInfo.mType)
                {
                    case GameSettingInfo.SETTING_TYPE.VOICE_STATUS:
                        player.Data.detailData.voiceOpen = System.BitConverter.ToBoolean(req.mInfo.mData,0);
                        break;
                }
                if(player.Data.detailData.roomid > 0)
                {
                    Gate2B_GameSettingRequest msg = new Gate2B_GameSettingRequest();
                    msg.mInfo = req.mInfo;
                    msg.mRoomId = player.Data.detailData.roomid;
                    msg.id = player.Id;
                    B2Gate_GameSettingResponse rps = await NetInnerComponent.Instance.GetByAppID(player.TemporaryData.BattleAppid).Call(msg) as B2Gate_GameSettingResponse;
                    response.errorCode = (int)ErrorCode.Success;
                    response.mInfo = rps.mInfo;
                }
                else
                {
                    response.errorCode = (int)ErrorCode.Success;
                    response.mInfo = req.mInfo;
                }
                //   player.TemporaryData.battleAppid;

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
