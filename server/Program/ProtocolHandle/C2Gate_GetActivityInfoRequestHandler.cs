using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(3304)]
    public class C2Gate_GetActivityInfoRequestHandler : AMRpcHandler<C2Gate_GetActivityInfoRequest>
    {
        protected override void Run(RpcPackage package)
        {
            C2Gate_GetActivityInfoRequest req = package.msg as C2Gate_GetActivityInfoRequest;
            Gate2C_GetActivityInfoResponse response = package.Response as Gate2C_GetActivityInfoResponse;

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
                package.Reply();
            }
        }
    }
}
