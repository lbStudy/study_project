using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(72774)]
    public class M2Gate_KickPlayerRequestHandler : AMRpcHandler<M2Gate_KickPlayerRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            M2Gate_KickPlayerRequest req = pacakage.msg as M2Gate_KickPlayerRequest;
            Gate2M_KickPlayerResponse response = pacakage.Response as Gate2M_KickPlayerResponse;

            try
            {
                if(req.playerids == null)
                {
                    return;
                }
                foreach(long v in req.playerids)
                {
                    Player player = PlayerManagerComponent.Instance.Find(v);
                    if (player == null)
                        continue;
                    player.TemporaryData.checkCode = "";                    
                    TranspondInfo ti = TranspondComponent.instance.Find(v);
                    if(ti != null)
                    {
                        if (ti.clientSession != null)
                            ti.clientSession.Dispose();
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
