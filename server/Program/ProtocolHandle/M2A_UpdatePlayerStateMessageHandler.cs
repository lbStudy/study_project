using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(18669)]
    public class M2A_UpdatePlayerStateMessageHandler : AMHandler<M2A_UpdatePlayerStateMessage>
    {
        protected override void Run(MsgPackage package)
        {
	    M2A_UpdatePlayerStateMessage msg = package.msg as M2A_UpdatePlayerStateMessage;
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
