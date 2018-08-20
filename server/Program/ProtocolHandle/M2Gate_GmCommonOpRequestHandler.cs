using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(5597)]
    public class M2Gate_GmCommonOpRequestHandler : AMRpcHandler<M2Gate_GmCommonOpRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            M2Gate_GmCommonOpRequest req = pacakage.msg as M2Gate_GmCommonOpRequest;
            Gate2M_GmCommonOpResponse response = pacakage.Response as Gate2M_GmCommonOpResponse;

            try
            {
                Player player = PlayerManagerComponent.Instance.Find(req.id);
                if(player == null)
                {
                    response.errorCode = (int)ErrorCode.NotExistPlayer;
                    return;
                }
                if(req.op == 1)
                {//Ôö¼Ó
                    if(req.param1 == 1)
                    {
                        player.CommonData.Add(D_AttributeType.roomcard, req.param2, true, LogAction.gm_fillcard);
                        player.CommonData.SynchroData();
                    }
                }
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
