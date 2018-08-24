using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(19483)]
    public class M2Gate_PlayerConnectGateNotifyRequestHandler : AMRpcHandler<M2Gate_PlayerConnectGateNotifyRequest>
    {
        protected override void Run(RpcPackage package)
        {
            M2Gate_PlayerConnectGateNotifyRequest req = package.msg as M2Gate_PlayerConnectGateNotifyRequest;
            Gate2M_PlayerConnectGateNotifyResponse response = package.Response as Gate2M_PlayerConnectGateNotifyResponse;

            try
            {
                VerifyInfo info = VerifyComponent.Instance.Find(req.id);
                if (info == null)
                {
                    info = VerifyComponent.Instance.Add(req.id);
                }
                response.checkcode = info.checkCode;
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
