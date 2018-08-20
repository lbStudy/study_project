using Base;
using Data;
using System;

namespace ProtocolHandle
{
    [Protocol(77109)]
    public class B2C_UseAtkMessageHandler : AMHandler<B2C_UseAtkMessage>
    {
        protected override void Run(MsgPacakage package)
        {
	    B2C_UseAtkMessage msg = package.msg as B2C_UseAtkMessage;
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
