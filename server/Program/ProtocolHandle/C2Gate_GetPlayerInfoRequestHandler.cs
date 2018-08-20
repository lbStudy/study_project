using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(75780)]
    public class C2Gate_GetPlayerInfoRequestHandler : AMRpcHandler<C2Gate_GetPlayerInfoRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            C2Gate_GetPlayerInfoRequest req = pacakage.msg as C2Gate_GetPlayerInfoRequest;
            Gate2C_GetPlayerInfoResponse response = pacakage.Response as Gate2C_GetPlayerInfoResponse;

            try
            {
                Player player = PlayerManagerComponent.Instance.Find(pacakage.Toid);
                if(player == null)
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
                }
                response.detail = player.Data.detailData;
                response.extra = player.Data.extraData;
                response.iconUrl = player.TemporaryData.iconUrl;
                response.sex = player.TemporaryData.sex;
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
