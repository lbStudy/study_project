using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(19483)]
    public class M2Gate_PlayerConnectGateNotifyRequestHandler : AMRpcHandler<M2Gate_PlayerConnectGateNotifyRequest>
    {
        protected override void Run(RpcPacakage pacakage)
        {
            M2Gate_PlayerConnectGateNotifyRequest req = pacakage.msg as M2Gate_PlayerConnectGateNotifyRequest;
            Gate2M_PlayerConnectGateNotifyResponse response = pacakage.Response as Gate2M_PlayerConnectGateNotifyResponse;

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
                pacakage.Reply();
            }
        }
    }
}
