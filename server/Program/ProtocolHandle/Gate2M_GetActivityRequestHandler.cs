using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(1686)]
    public class Gate2M_GetActivityRequestHandler : AMRpcHandler<Gate2M_GetActivityRequest>
    {
        protected override void Run(RpcPackage package)
        {
            Gate2M_GetActivityRequest req = package.msg as Gate2M_GetActivityRequest;
            M2Gate_GetActivityResponse response = package.Response as M2Gate_GetActivityResponse;

            try
            {
                if(Game.Instance.IsFinishModule(InitModule.GetActivity))
                {
                    response.activitys = GmActivityManagerComponent.Instance.activitys;
                }
                else
                {
                    response.errorCode = (int)ErrorCode.Fail;
                    return;
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
