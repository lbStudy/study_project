using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(48870)]
    public class Gate2M_RemovePlayerRequestHandler : AMRpcHandler<Gate2M_RemovePlayerRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            Gate2M_RemovePlayerRequest req = pacakage.msg as Gate2M_RemovePlayerRequest;
            M2Gate_RemovePlayerResponse response = pacakage.Response as M2Gate_RemovePlayerResponse;
            try
            {
                for(int i = 0; i <req.playerids.Count; i++)
                    ServerAllotComponent.Instance.PlayerRemove(req.playerids[i]);
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
