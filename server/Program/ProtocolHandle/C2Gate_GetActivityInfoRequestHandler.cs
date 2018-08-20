using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(3304)]
    public class C2Gate_GetActivityInfoRequestHandler : AMRpcHandler<C2Gate_GetActivityInfoRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            C2Gate_GetActivityInfoRequest req = pacakage.msg as C2Gate_GetActivityInfoRequest;
            Gate2C_GetActivityInfoResponse response = pacakage.Response as Gate2C_GetActivityInfoResponse;

            try
            {
                response.activitys = null;//ActivityManagerComponent.Instance.Activitys;
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
