using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(48870)]
    public class Gate2M_RemovePlayerRequestHandler : AMRpcHandler<Gate2M_RemovePlayerRequest>
    {
        protected override void Run(RpcPackage package)
        {
            Gate2M_RemovePlayerRequest req = package.msg as Gate2M_RemovePlayerRequest;
            M2Gate_RemovePlayerResponse response = package.Response as M2Gate_RemovePlayerResponse;
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
                package.Reply();
            }
        }
    }
}
