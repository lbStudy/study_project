using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(80544)]
    public class Gate2M_UpdateServerStateMessageHandler : AMHandler<Gate2M_UpdateServerStateMessage>
    {
        protected override void Run(MsgPackage package)
        {
	    Gate2M_UpdateServerStateMessage msg = package.msg as Gate2M_UpdateServerStateMessage;
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
