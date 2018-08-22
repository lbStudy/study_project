using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(33968)]
    public class A2A_ServerRegisterRequestHandler : AMRpcHandler<A2A_ServerRegisterRequest>
    {
        protected override void Run(RpcPacakage package)
        {
		A2A_ServerRegisterRequest req = package.msg as A2A_ServerRegisterRequest;
		A2A_ServerRegisterResponse response = package.Response as A2A_ServerRegisterResponse;
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