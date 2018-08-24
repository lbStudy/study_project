using Base;
using Data;
using System;
using System.Collections.Generic;

namespace ProtocolHandle
{
    [Protocol(44677)]
    public class M2Gate_GmFindPlayerInfoRequestHandler : AMRpcHandler<M2Gate_GmFindPlayerInfoRequest>
    {
        protected override void Run(RpcPackage package)
        {
            M2Gate_GmFindPlayerInfoRequest req = package.msg as M2Gate_GmFindPlayerInfoRequest;
            Gate2M_GmFindPlayerInfoResponse response = package.Response as Gate2M_GmFindPlayerInfoResponse;

            try
            {
                response.playerdatas = new List<PlayerDetailData>();
                for(int i = 0; i < req.playerids.Count; i++)
                {
                    Player player = PlayerManagerComponent.Instance.Find(req.playerids[i]);
                    if(player != null)
                    {
                        response.playerdatas.Add(player.Data.detailData);
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
                package.Reply();
            }
        }
    }
}
