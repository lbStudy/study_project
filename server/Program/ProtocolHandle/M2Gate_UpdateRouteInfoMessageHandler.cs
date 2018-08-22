using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(8504)]
    public class M2Gate_UpdateRouteInfoMessageHandler : AMHandler<M2Gate_UpdateRouteInfoMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    M2Gate_UpdateRouteInfoMessage msg = package.msg as M2Gate_UpdateRouteInfoMessage;
	    try
	    {

	    }
	    catch(Exception e)
	    {
		Console.WriteLine(e.ToString());
	    }
	    finally
	    {
		package.Clear();
	    }
        }
    }
}
