using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(78687)]
    public class C2B_DecAroundCharacterRequestHandler : AMRpcHandler<C2B_DecAroundCharacterRequest>
    {
        protected override void Run(RpcPacakage package)
        {
            C2B_DecAroundCharacterRequest req = package.msg as C2B_DecAroundCharacterRequest;
            B2C_DecAroundCharacterResponse response = package.Response as B2C_DecAroundCharacterResponse;
            try
            {

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
