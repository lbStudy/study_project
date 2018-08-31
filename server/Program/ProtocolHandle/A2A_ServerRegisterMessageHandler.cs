using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(65357)]
    public class A2A_ServerRegisterMessageHandler : AMHandler<A2A_ServerRegisterMessage>
    {
        protected override void Run(MsgPackage package)
        {
	    A2A_ServerRegisterMessage msg = package.msg as A2A_ServerRegisterMessage;
	    try
	    {

	    }
	    catch(Exception e)
	    {
		Console.WriteLine(e.ToString());
	    }
	    finally
	    {
		package.Dispose();
	    }
        }
    }
}
