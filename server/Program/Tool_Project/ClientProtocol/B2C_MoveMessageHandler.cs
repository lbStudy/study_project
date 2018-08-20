using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(20672)]
    public class B2C_MoveMessageHandler : AMHandler<B2C_MoveMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_MoveMessage msg = package.msg as B2C_MoveMessage;
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
